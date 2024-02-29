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
        public static async Task<IResult> Action
            (LoginRequest loginRequest,
            IConfiguration configuration ,
            UserManager<IdentityUser> userManage,
            ILogger<TokenPost> log, //ILogger<TokenPost> -> O .Net ja tem um logger por default, para usarmos ele é só passar o ILogger<TokenPost> como parametro a tipagem dele é a classe que estamos usando
            IWebHostEnvironment environment) //IWebHostEnvironment -> para pegar o ambiente(stage ou prod) que estamos rodando
        {
            log.LogInformation("Pegand o token"); //-> Ai basta a gente dizer o que queremos.
            log.LogWarning("Warning");
            log.LogError("Error");

            /*
             Informação importante sobre os logs..
             No program.cs temos a configuração do log, onde podemos definir o nivel de log que queremos, por exemplo:
            "Logging": {
            "LogLevel": {
              "Default": "Information",
              "Microsoft.AspNetCore": "Warning"
            }
           },     
            
            Se eu trocar "Information" por "Error" ou "Warning" por exemplo, todos os logs que eu coloquei como "Information" não vão aparecer, pois o nivel de log foi definido como "Error" e com isso só os de Error vao aparecer.
             "Information" -> ele mostra todos os logs independente se é error, warning, information
             */

            var user = await userManage.FindByEmailAsync(loginRequest.Email);
            if(user == null)
            {
                return Results.Unauthorized();
            }   

            if(!await userManage.CheckPasswordAsync(user, loginRequest.Password))
            {
                return Results.Unauthorized();
            }

            var claims = await userManage.GetClaimsAsync(user);
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
                Expires = environment.IsDevelopment() || environment.IsStaging() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddHours(1), //-> tanto para desenvolvimento ou staging é 1 ano, se nao for nenhum dos dois ele vai saber que é de produção e vai ser 1 hora
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
