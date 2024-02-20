using Microsoft.AspNetCore.Mvc;
using OrderApi.Domain.Products;
using OrderApi.Infra.Data;

namespace OrderApi.Entpoints.Categories
{
    //cada arquivo sera responsavel por apenas 1 rota
    public class CategoryPut
    {
        public static string Template => "api/v1/PutCategories/{id}";
        public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
        public static Delegate Handle => Action;

        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc
        public static IResult Action([FromRoute]Guid id, CategoryRequest categoryRequest, ApplicationDbContext context)
        {
            var category = context.Categories.Where(c => c.Id == id).FirstOrDefault();

            category.Name = categoryRequest.Name;
            category.Active = categoryRequest.Active;

            context.SaveChanges();

            //retornando 201 e retornando o id do objeto criado
            return Results.Ok();
        }
    }
}
