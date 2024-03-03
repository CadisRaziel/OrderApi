using Microsoft.AspNetCore.Authorization;
using OrderApi.Domain.Orders;
using OrderApi.Domain.Products;
using OrderApi.Infra.Data;
using OrderApi.Utils.ErrosEndpoint;
using System.Security.Claims;

namespace OrderApi.Endpoints.Orders
{
    public class OrderPost
    {
        public static string Template => "api/v1/order";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "CpfPolicy")] //-> qualquer usuario com token e com um cpf consegue fazer a requisição

        public static async Task<IResult> Action(OrderRequest orderRequest, HttpContext http, ApplicationDbContext context)
        {
            //Pegando o Id e nome do client (esta no token, por isso podemos pegar das claims)
            var clientId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var clientName = http.User.Claims.First(c => c.Type == "Name").Value;

            /*
             //Eu poderia colocar validações aqui igual abaixo
             //Porém no 'domain' de 'Order' eu ja possuo essas mesmas validações la
            //Essas validações aqui mesmo dando nullo ou nao tendo produto ou deliveryAddress, ele vai pesquisar no banco de dados


             //caso nao tenha produto
             if(orderRequest.ProductIds == null || !orderRequest.ProductIds.Any())
             {
                 return Results.BadRequest("No product in the order");
             }

             //caso nao tenha endereço de entrega
             if (string.IsNullOrEmpty(orderRequest.DeliveryAddress))
             {
                 return Results.BadRequest("Delivery address is required");
             }
             */

            //Dessa maneira abaixo ele só vai no banco de dados se o productId for diferente de nulo e se tiver produto
            List<Product> productsFound = null;
            if(orderRequest.ProductIds != null || orderRequest.ProductIds.Any())
            {
                //Se ele nao for nulo e se ele tiver algum produtoId ele vai no banco de dados, caso contrario ele nao vai no banco de dados
                productsFound = context.Products.Where(p => orderRequest.ProductIds.Contains(p.Id)).ToList();
            }

            /*
              var products = new List<Product>();
              foreach (var productId in orderRequest.ProductIds)
             {
                 //Imagine que o client vai comprar 10 produtos, então aqui "orderRequest.ProductIds" eu tenho 10 id de produto
                 //Para cada id de produto, eu vou no banco de dados e pego o produto correspondente, ou seja, vou 10 vezes no banco de dados
                 var product = context.Products.First(p => p.Id == productId);
                 if (product == null)
                 {
                     return Results.NotFound($"Product with id {productId} not found");
                 }
                 products.Add(product);
             }

            //Para evitar isso vamos utilizar a variavel "productsFound" que vai armazenar todos os produtos de uma vez só, ou seja traz tudo de uma vez só se for encontrado e nao 1 por 1 igual o foreach
             */

           

            var order = new Order(clientId, clientName, productsFound, orderRequest.DeliveryAddress);

            if (!order.IsValid)
            {
                return Results.ValidationProblem(order.Notifications.ConvertToProblemDetails());
            }

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

           return Results.Created($"/api/v1/order/{order.Id}", order.Id);
        }
    }
}
