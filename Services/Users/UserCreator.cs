using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace OrderApi.Services.Users
{
    public class UserCreator
    {

        private readonly UserManager<IdentityUser> _userManage;
        public UserCreator(UserManager<IdentityUser> userManage)
        {
            _userManage = userManage;
        }

        //Task<(IdentityResult, string) -> Quando colocamos 2 tipos entre parenteses, estamos dizendo que o metodo retorna uma tupla (ou seja ele vai retornar 2 valores)
        public async Task<(IdentityResult, string)> Create(string email, string password, List<Claim> claims)
        {
            var newUser = new IdentityUser //-> como eu preciso retornar tambem esse newUser eu criei uma 'tupla' de retorno, olhe no comentario de cima
            {
                Email = email,
                UserName = email
            };
            var result = await _userManage.CreateAsync(newUser, password);

            if (!result.Succeeded)
            {
                return (result, String.Empty); //Como agora eu tenho 2 retornos eu coloco entre parenteses e passo os dois
            }
   
            return (await _userManage.AddClaimsAsync(newUser, claims), newUser.Id); //Como agora eu tenho 2 retornos eu coloco entre parenteses e passo os dois
        }
    }
}
