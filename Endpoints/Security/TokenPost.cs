using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderApi.Endpoints.Security
{
    public class TokenPost
    {
        public static string Template => "api/v1/token";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous] //-> para permitir que qualquer um acesse essa rota

        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc
        public static IResult Action(LoginRequest loginRequest,IConfiguration configuration , UserManager<IdentityUser> userManage) 
        {
            var user = userManage.FindByEmailAsync(loginRequest.Email).Result;
            if(user == null)
            {
                return Results.Unauthorized();
            }   

            if(!userManage.CheckPasswordAsync(user, loginRequest.Password).Result)
            {
                return Results.Unauthorized();
            }

            var claims = userManage.GetClaimsAsync(user).Result;
            var subject = new ClaimsIdentity(new Claim[] //-> arrays de claim
                {
                    new Claim(ClaimTypes.Email, loginRequest.Email), //-> quem for obter esse token pode ler o email que esta dentro do token                
                    new Claim(ClaimTypes.NameIdentifier, user.Id), //-> quem for obter esse token pode ler o id do usuario  
                });
            subject.AddClaims(claims); //-> adicionando as claims ao token (adicionando todos os claims que criamos)

            /*
            key -> Assinatura de tokens:
            Ao gerar um token de segurança, geralmente você o assina digitalmente para garantir sua integridade e autenticidade. 
            Isso significa que você usa uma chave secreta conhecida apenas pelo emissor e pelo receptor do token para criar uma 
            assinatura criptográfica do conteúdo do token. Para fazer isso, a chave secreta precisa ser convertida em bytes, o
            que é feito aqui usando Encoding.ASCII.GetBytes

            Quando um token é recebido e precisa ser validado, a mesma chave secreta é usada para verificar a assinatura do token
            Isso garante que o token não tenha sido alterado e que tenha sido realmente emitido pelo emissor legítimo. Para fazer
            essa verificação, a chave secreta também precisa ser fornecida como bytes.
             */
            var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]); 
            var tokenDescriptor = new SecurityTokenDescriptor //SecurityTokenDescriptor -> responsavel por gerar o token
            {
                Subject = subject,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature), 
                Audience = configuration["JwtBearerTokenSettings:Audience"], //-> Este parâmetro define para quem o token é destinado ou quem pode usar o token(quem utilizar esse banco)
                Issuer = configuration["JwtBearerTokenSettings:Issuer"], //-> Este parâmetro define quem está assinando (emitindo) o token
                Expires = DateTime.UtcNow.AddHours(1), //-> tempo de expiração do token. Por exemplo, se a aplicação está emitindo tokens de autenticação e identificação para o sistema, então o valor do Issuer pode ser definido como "Issuer".
            };

            var tokenHandler = new JwtSecurityTokenHandler(); //-> gera o manipulador do token
            var token = tokenHandler.CreateToken(tokenDescriptor); //-> atravez da descrição do token criada acima, ele cria o token
            return Results.Ok(new
            {
                token = tokenHandler.WriteToken(token) //-> retorna o token
            });
        }
    }
}
