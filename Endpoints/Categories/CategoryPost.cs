using OrderApi.Domain.Products;
using OrderApi.Infra.Data;
using OrderApi.Utils.ErrosEndpoint;

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
         
           /*
            Tipo de validação que pode ser feita (validar o campo de um a um
            if (string.IsNullOrEmpty(categoryRequest.Name))
            {
                return Results.BadRequest("Name is required");
            }
            */


            //Ou podemos adicionar o package 'Flunt' para fazer a validação direto no objeto modelo Entity
            var category = new Category(categoryRequest.Name, "test created", "test edited") //-> passando a validação pelo construtor
            {
                //Name = categoryRequest.Name, //-> eu removo daqui e coloco no construtor, isso serve para todos que esta no construtor    
            };

            //Validando se o objeto é valido
            if(!category.IsValid)
            {                
                return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
            }

            context.Categories.Add(category);
            context.SaveChanges();

            //retornando 201 e retornando o id do objeto criado
            return Results.Created($"/api/v1/categories/{category.Id}", category.Id);
        }
    }
}
