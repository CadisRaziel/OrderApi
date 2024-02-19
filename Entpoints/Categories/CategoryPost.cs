using OrderApi.Domain.Products;
using OrderApi.Infra.Data;

namespace OrderApi.Entpoints.Categories
{
    //cada arquivo sera responsavel por apenas 1 rota
    public class CategoryPost
    {
        public static string Template => "api/v1/categories";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc
        public static IResult Action(CategoryRequest categoryRequest, ApplicationDbContext context)
        {
            var category = new Category
            {
                Name = categoryRequest.Name
            };

            context.Categories.Add(category);
            context.SaveChanges();

            //retornando 201 e retornando o id do objeto criado
            return Results.Created($"/api/v1/categories/{category.Id}", category.Id);
        }
    }
}
