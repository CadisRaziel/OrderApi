using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Policy = "EmployeePolicy")]

        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc
        public static async Task<IResult> Action(EmployeeRequest employeeRequest, HttpContext http ,UserManager<IdentityUser> userManage) //UserManager<IdentityUser> -> serviço que gerencia a criaçao do usuario no banco
        {

            // Por causa da Policy aqui eu ja sei que o usuario esta autenticado !!
            var userId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value; //-> pegando o id do usuario logado
            var newUser = new IdentityUser
            {
                Email = employeeRequest.Email,
                UserName = employeeRequest.Email
            };

            var result = await userManage.CreateAsync(newUser, employeeRequest.Password);

            if (!result.Succeeded)
            {
                return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
            }

            //Ao inves de adicionar direto no banco, eu posso adicionar um novo atributo ao meu usuario aqui com a Claim
            //"EmployeeCode" -> chave criado no EmployeeRequest
            var userClaims = new List<Claim>
            {
                new Claim("EmployeeCode", employeeRequest.EmployeeCode),
                new Claim("Name", employeeRequest.Name),
                new Claim("CreatedBy", userId)
            };

            var claimResult = await userManage.AddClaimsAsync(newUser, userClaims);
                     

            if (!claimResult.Succeeded)
            {
                return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
            }

            //retornando 201 e retornando o id do objeto criado
            return Results.Created($"/api/v1/employees/{newUser.Id}", newUser.Id);
        }
    }
}
