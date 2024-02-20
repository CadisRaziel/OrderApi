using Microsoft.AspNetCore.Mvc;
using OrderApi.Domain.Products;
using OrderApi.Infra.Data;
using OrderApi.Utils.ErrosEndpoint;

namespace OrderApi.Entpoints.Categories
{
    //cada arquivo sera responsavel por apenas 1 rota
    public class CategoryPut
    {
        public static string Template => "api/v1/PutCategories/{id:guid}"; //-> adicionando :guid eu evito que ao usario colocar um id invalido apareça uma msg gigante(porém ele nao mostra nenhuma msg de erro e só 404)
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc
        public static IResult Action([FromRoute]Guid id, CategoryRequest categoryRequest, ApplicationDbContext context)
        {
            var category = context.Categories.Where(c => c.Id == id).FirstOrDefault();

            if(category == null)
            {
                return Results.NotFound();
            }

            category.EditInfo(categoryRequest.Name, categoryRequest.Active);

            if(!category.IsValid)
            {
                return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
            }   

            context.SaveChanges();

            //retornando 201 e retornando o id do objeto criado
            return Results.Ok();
        }
    }
}
