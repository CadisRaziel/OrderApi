using Microsoft.AspNetCore.Identity;
using OrderApi.Utils.ErrosEndpoint;
using System.Security.Claims;

namespace OrderApi.Endpoints.Employees
{
    public class EmployeePost
    {
        public static string Template => "api/v1/employees";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc
        public static IResult Action(EmployeeRequest employeeRequest, UserManager<IdentityUser> userManage) //UserManager<IdentityUser> -> serviço que gerencia a criaçao do usuario no banco
        {

            var user = new IdentityUser
            {
                Email = employeeRequest.Email,
                UserName = employeeRequest.Email
            };

            var result = userManage.CreateAsync(user, employeeRequest.Password).Result;

            //Ao inves de adicionar direto no banco, eu posso adicionar um novo atributo ao meu usuario aqui com a Claim
            //"EmployeeCode" -> chave criado no EmployeeRequest
            var userClaims = new List<Claim>
            {
                new Claim("EmployeeCode", employeeRequest.EmployeeCode),
                new Claim("Name", employeeRequest.Name)
            };

            var claimResult = userManage.AddClaimsAsync(user, userClaims).Result;
                     

            if (!claimResult.Succeeded)
            {
                return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
            }

            //retornando 201 e retornando o id do objeto criado
            return Results.Created($"/api/v1/employees/{user.Id}", user.Id);
        }
    }
}
