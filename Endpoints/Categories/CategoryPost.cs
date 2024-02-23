using Microsoft.AspNetCore.Authorization;
using OrderApi.Domain.Products;
using OrderApi.Infra.Data;
using OrderApi.Utils.ErrosEndpoint;
using System.Security.Claims;

namespace OrderApi.Entpoints.Categories
{
    //cada arquivo sera responsavel por apenas 1 rota
    public class CategoryPost
    {
        public static string Template => "api/v1/categories";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        //[AllowAnonymous] //-> para permitir que qualquer um acesse essa rota
        [Authorize(Policy = "EmployeePolicy")] //-> para permitir que apenas usuarios logados(Authenticados) acessem essa rota
            
        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc
        public static async Task<IResult> Action(CategoryRequest categoryRequest, HttpContext http ,ApplicationDbContext context)
        {

            /*
             Tipo de validação que pode ser feita (validar o campo de um a um
             if (string.IsNullOrEmpty(categoryRequest.Name))
             {
                 return Results.BadRequest("Name is required");
             }
             */

            //Por causa da Policy aqui eu ja sei que o usuario esta autenticado !!
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value; //-> pegando o id do usuario logado


            //Ou podemos adicionar o package 'Flunt' para fazer a validação direto no objeto modelo Entity
            var category = new Category(categoryRequest.Name, userId, userId) //-> passando a validação pelo construtor (2x userId pq quem criou foi tambem quem editou)
            {
                //Name = categoryRequest.Name, //-> eu removo daqui e coloco no construtor, isso serve para todos que esta no construtor    
            };

            //Validando se o objeto é valido
            if(!category.IsValid)
            {                
                return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
            }

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            //retornando 201 e retornando o id do objeto criado
            return Results.Created($"/api/v1/categories/{category.Id}", category.Id);
        }
    }
}
