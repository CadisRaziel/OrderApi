using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OrderApi.Endpoints.Employees;
using OrderApi.Infra.Service;
using OrderApi.Services.Users;
using static System.Net.WebRequestMethods;
using System.Security.Claims;

namespace OrderApi.Endpoints.Clients
{
    public class ClientGet
    {
        public static string Template => "api/v1/clientGet";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;
        [AllowAnonymous]

        public static async Task<IResult> Action(HttpContext http)
        {
            //var user = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value; //-> verifica o usuario logado
            var user = http.User;
            var result = new
            {
                //objeto anonimo -> nao tem um tipo definido
                //é criado em tempo de execução
                Id = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value, //NameIdentifier -> pois na hora de cria o token eu passo esse parametro na claim(value é o id)
                Name = user.Claims.First(c => c.Type == "Name").Value, //Name -> pois na hora de cria o token eu passo esse parametro na claim(value é o name)
                Cpf = user.Claims.First(c => c.Type == "Cpf").Value,  //Cpf -> pois na hora de cria o token eu passo esse parametro na claim(value é o name)
            };

            return Results.Ok(result);
        }
    }
}
