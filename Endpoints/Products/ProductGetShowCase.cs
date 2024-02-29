using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderApi.Infra.Data;

namespace OrderApi.Endpoints.Products
{
    public class ProductGetShowCase
    {
        public static string Template => "api/v1/product/showcase";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous]
        public static IResult Action(ApplicationDbContext context, int page = 1, int row = 10, string orderBy = "name") //-> passando parametros 'default' no parametro de entrada (esse parametros com valor default eles devem ficar por ultimo na ordem de parametros !)
        {         
            
            //Definindo o numero de 'rows' que o usuario ou dev front pode pedir
            //Rows -> quantidade de itens que vai ter em uma pagina.
            if (row > 10)
            {
                return Results.Problem(title: "Invalid row", detail: "The maximum number of rows is 10", statusCode: 400);
            }

            //IQueryable -> QueryBase é um IQueryable então eu posso ir montando a query sem que ele va no banco de dados, ele só vai no banco quando eu por um ".toList()" ou ".FirstOrDefault()" no final
            //AsNoTracking() -> é um metodo que diz para o entity framework que eu não vou alterar os dados que eu estou pegando, então ele não vai ficar monitorando esses dados
            //AsNoTracking() -> ou seja se eu for fazer somente consulta no banco de dados é interessante eu colocar ele(ele nao fica rastreado na memoria e nos da mais performance)
            var queryBase = context.Products.AsNoTracking().Include(p => p.Category)
                .Where(p => p.HasStock && p.Category.Active);

            //ordenar a consulta pelo nome ou preço
            if (orderBy == "name")
            {
                queryBase = queryBase.OrderBy(p => p.Name);
            }
            else if (orderBy == "price")
            {
                queryBase = queryBase.OrderBy(p => p.Price);
            } else
            {
                //Tratativa para caso não o dev front ou usuario tente fitlrar por outra coisa que nao seja name ou price
                return Results.Problem(title: "OrderBy", detail: "Order only by price or name", statusCode: 400);
            }

            //filtrando a consulta por paginação
            var queryFilter = queryBase.Skip((page - 1) * row).Take(row); 

                     
           
            var products = queryFilter.ToList(); //-> com esse ToList() ele vai no banco de dados e pega os dados
            

            var results = products.Select(p => new ProductResponse(p.Id, p.Name, p.Category.Name, p.Description, p.Price, p.HasStock, p.Active));
            return Results.Ok(results);
        }
    }
}
