using Microsoft.AspNetCore.Authorization;
using OrderApi.Domain.Products;
using OrderApi.Infra.Data;

namespace OrderApi.Entpoints.Categories
{
    //cada arquivo sera responsavel por apenas 1 rota
    public class CategoryGetAll
    {
        public static string Template => "api/v1/getCategories";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize]

        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc
        public static IResult Action(ApplicationDbContext context)
        {
            var categories = context.Categories.ToList();    
            

            //Nesse momento o linq do c# vai selecionar 1 a 1 dentro dessa lista e vai criar um novo objeto com CategoryResponse (ao inves de retornar a lista entity Category eu retorno meu CategoryResponse personalizado somente com os itens que eu quero retornar)
            var reponse = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Active = c.Active
            });

            return Results.Ok(reponse);
        }
    }
}
