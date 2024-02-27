using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Products;
using OrderApi.Entpoints.Categories;
using OrderApi.Infra.Data;
using OrderApi.Utils.ErrosEndpoint;
using System.Security.Claims;

namespace OrderApi.Endpoints.Products
{
    public class ProductPost
    {
        public static string Template => "api/v1/product";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        //[AllowAnonymous] //-> para permitir que qualquer um acesse essa rota
        [Authorize(Policy = "EmployeePolicy")] //-> para permitir que apenas usuarios logados(Authenticados) acessem essa rota

        public static async Task<IResult> Action(ProductRequest productRequest, HttpContext http, ApplicationDbContext context)
        {
                     
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == productRequest.CategoryId);
            var product = new Product(productRequest.Name, category, productRequest.Description, productRequest.HasStock, productRequest.Price, userId);

            //Validando se o objeto é valido
            if (!category.IsValid)
            {
                return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
            }

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            //retornando 201 e retornando o id do objeto criado
            return Results.Created($"/api/v1/categories/{product.Id}", product.Id);
        }
    }
}
