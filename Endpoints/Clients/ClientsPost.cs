using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OrderApi.Endpoints.Employees;
using OrderApi.Services.Users;
using OrderApi.Utils.ErrosEndpoint;
using System.Security.Claims;

namespace OrderApi.Endpoints.Clients
{
    public class ClientsPost
    {
        public static string Template => "api/v1/client";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous] //-> para permitir que qualquer um acesse essa rota


        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc

        /*
         Metodo antigo antes da refatoração
         public static async Task<IResult> Action(ClientsRequest clientsRequest, HttpContext http, UserManager<IdentityUser> userManage) //UserManager<IdentityUser> -> serviço que gerencia a criaçao do usuario no banco
         {


             var newUser = new IdentityUser
             {
                 Email = clientsRequest.Email,
                 UserName = clientsRequest.Email
             };
             var result = await userManage.CreateAsync(newUser, clientsRequest.Password);

             if (!result.Succeeded)
             {
                 return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
             }

              //Pelo claim eu sei se é usuario ou empregado
             //empregado tem 'Employee' e usuario tem 'Cpf'
             var userClaims = new List<Claim>
             {
                 new Claim("Cpf", clientsRequest.Cpf),
                 new Claim("Name", clientsRequest.Name)              
             };

             var claimResult = await userManage.AddClaimsAsync(newUser, userClaims);


             if (!claimResult.Succeeded)
             {
                 return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
             }

             //retornando 201 e retornando o id do objeto criado
             return Results.Created($"/api/v1/client/{newUser.Id}", newUser.Id);
         }
         */

        //Metodo novo depois da refatoração
        public static async Task<IResult> Action(ClientsRequest clientsRequest, UserCreator userCreator)
        {
            var userClaims = new List<Claim>
            {
                 //Pelo claim eu sei se é usuario ou empregado
                 //empregado tem 'Employee' e usuario tem 'Cpf'
                new Claim("Cpf", clientsRequest.Cpf),
                new Claim("Name", clientsRequest.Name)
            };

            //Pegando o retorno da 'Tupla' da classe 'UserCreator' no metodo 'Create'
            //com isso eu tipo a variavel com os dois retornos  (IdentityResult resultIdentity, string userId) usando parenteses
            (IdentityResult resultIdentity, string userId) result = await userCreator.Create(clientsRequest.Email, clientsRequest.Password, userClaims);

            if (!result.resultIdentity.Succeeded) //-> aqui eu pego o erro de 1 retorno só, apenas do resultIdentity (com isso eu vejo se o meu primeiro retorno foi sucesso ou nao)
            {
                return Results.ValidationProblem(result.resultIdentity.Errors.ConvertToProblemDetails());
            }            

            return Results.Created($"/api/v1/client/{result.userId}", result.userId); //-> aqui eu uso meu segundo retorno, o userId
        }
    }
}
