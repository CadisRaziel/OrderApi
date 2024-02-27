using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderApi.Entpoints.Categories;
using OrderApi.Infra.Data;

namespace OrderApi.Endpoints.Products
{
    public class ProductGetAll
    {
        public static string Template => "api/v1/getProducts";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]

        public static IResult Action(ApplicationDbContext context)
        {
            /*             
             Include() -> é usado para especificar entidades relacionadas que você deseja incluir na consulta. 
             No seu exemplo, p => p.Category está especificando que você deseja incluir a entidade de categoria relacionada aos produtos.

             
             Em uma API em .NET, quando você está consultando dados de um banco de dados usando o Entity Framework,
             o método Include() é usado para especificar entidades relacionadas que você deseja incluir na consulta.
             No seu exemplo, p => p.Category está especificando que você deseja incluir a entidade de categoria relacionada aos produtos.

             Isso é conhecido como "carregamento antecipado" (eager loading), o que significa que além dos produtos, 
             você também está solicitando que o Entity Framework carregue as categorias associadas a esses produtos em
             uma única consulta ao banco de dados. Isso é útil quando você sabe que vai precisar dos dados relacionados 
             e deseja evitar consultas adicionais ao banco de dados, melhorando o desempenho da sua aplicação.
             */

            var products = context.Products.Include(p => p.Category).OrderBy(p => p.Name).ToList(); //-> Aqui eu pego o nome da categoria na tabela de categoria
            var results = products.Select(p => new ProductResponse(p.Id, p.Name, p.Category.Name, p.Description, p.Price, p.HasStock, p.Active));
            return Results.Ok(results);
        }
    }
}
