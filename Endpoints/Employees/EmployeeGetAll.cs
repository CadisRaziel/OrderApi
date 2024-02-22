using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using OrderApi.Infra.Service;
using System.Security.Claims;

namespace OrderApi.Endpoints.Employees
{
    public class EmployeeGetAll
    {
        public static string Template => "api/v1/employeesGetAll";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        /*
         Roles (Funções): Uma Role é uma maneira de agrupar usuários que têm permissões semelhantes dentro de um sistema. 
         Por exemplo, você pode ter roles como "Admin", "Usuário", "Gerente", etc. Os usuários podem ser atribuídos a 
         uma ou mais roles e, em seguida, as permissões associadas a essas roles são aplicadas aos usuários.

         Policy (Política): Uma política é uma maneira mais flexível e poderosa de definir permissões e regras de autorização. 
         Em vez de apenas agrupar usuários por funções predefinidas, as políticas permitem que você defina condições mais 
         complexas para determinar se um usuário tem permissão para acessar um recurso ou executar uma ação. As políticas podem
         ser baseadas em roles, mas também podem depender de outros fatores, como propriedades do usuário, contexto da solicitação, etc.
         */
        [Authorize(Policy = "EmployeePolicy")] 
        //[Authorize(Policy = "Employee0050Policy")] //-> Aqui somente o usuario com EmployeeCode0050 tera acesso a essa rota

        //IResult -> para dizer se deu 200 - 201 - 400 - 404 ... etc



        #region
        //--------- CÒDIGO ANTIGO SEM O DAPPER  ---------   
        /*
        public static IResult Action(int page, int rows, UserManager<IdentityUser> userManage) //UserManager<IdentityUser> -> serviço que gerencia a criaçao do usuario no banco
        {
                       
            //--------- Explicação da paginação  ---------
            //(page - 1) * rows ) -> Estou na pagina1, vou fazer pagina1 - 1 que vai dar 0, agora a quantidade de rows imagine que é 10, entao vai ser 0 * 10 = 0
            //Na primeira paginação eu nao pulo nenhuma linha
            //Take(rows) -> ja no take eu sempre vou pegar 10 registros


            //(page - 1) * rows: Esta parte é responsável por calcular quantos itens devem ser "pulados" antes de começar a seleção dos dados da página atual.
            //Quando page é 1, isso resultará em 0, indicando que nenhum item precisa ser pulado, o que faz sentido para a primeira página.

            //.Take(rows): Esta parte é responsável por selecionar os próximos rows (ou seja, a quantidade de linhas ou registros por página) após os itens que foram pulados. Como você mencionou,
            //isso garantirá que sempre sejam selecionados os registros corretos para a página atual.

            //ToList(): Finalmente, a chamada ToList() faz com que a consulta seja executada e os resultados sejam materializados em uma lista de usuários
            //--------- Explicação da paginação  ---------
            

            var users = userManage.Users.Skip((page - 1) * rows).Take(rows).ToList();
            var employees = new List<EmployeeResponse>();
            foreach (var item in users)
            //Cuidado ao realizar o foreach pois a cada passada no users/claim ele vai no banco, imagine um banco com 5 mil usuarios, vai ser relizado 5 mil consultas, solução: Paginação
            {
                var claims = userManage.GetClaimsAsync(item).Result;
                var claimName = claims.FirstOrDefault(c => c.Type == "Name"); //->Name -> pois no post nos definimos a chave como 'Name'
                var userName = claimName != null ? claimName.Value : string.Empty;
                employees.Add(new EmployeeResponse(item.Id, item.Email, userName));
            }

            //retornando 201 e retornando o id do objeto criado
            return Results.Ok(employees);
        }
        */
        //--------- CÒDIGO ANTIGO SEM O DAPPER  ---------   
        #endregion


        #region
        //--------- CÒDIGO NOVO COM O DAPPER  ---------
        /*
        public static IResult Action(int? page, int? rows, IConfiguration configuration) //IConfiguration -> Com o dapper nos pegamos direto do Iconfiguration(ele busca no program.cs a configuração "builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:OrderApiDb"]);")
        {
            //OBS -> Com o dapper eu nao preciso realizar paginação, porém se forem muuuuuuuuuuutios dados eu ainda preciso dela (porém posso deixar opcional colcoando ? nos parametros)!!
            //Tambem temos a facilidade de juntar tabelas pra trazer dados que queremos, com o EF é mais complicado
            //Dapper -> MicroRM desenvolvido para termos performace para grandes volumes de dados (no exemplo acima imagina se tivessemos 5 mil usuarios, seria uma dor de cabeça)
            //EF x Dapper -> faz um tipo de conexão com o banco de dados - Dapper faz outro tipo de conexão com o banco de dados
            

            var db = new SqlConnection(configuration["ConnectionStrings:OrderApiDb"]); //-> Aqui eu pego a string de conexão que esta no appsettings.json

            //-> Aqui eu faço a consulta no banco de dados e retorno uma lista de EmployeeResponse (repare que sem o EF as consultas tende a ser manuais, por isso é mais rapido)
            //Não preciso ficar fazendo consulta aqui no código e testando, eu posso realizar a consulta direto no azure database e depois trazer pra ca
            //@ -> para escrever em mais de uma linha sem precisar concatenar
            //O dapper converte a consulta em uma classe que é a tipagem dele, aqui no caso é EmployeeResponse (ou seja ele vai converter as colunas para a propriedade da classe que estamos passando la
            var query =
                @"  SELECT Email, ClaimValue as Name
                    FROM AspNetUsers apelidoUser INNER JOIN AspNetUserClaims apelidoClaims
                    on apelidoUser.Id = apelidoClaims.UserId and ClaimType = 'Name'
                    ORDER BY Name
                    OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY
                 ";
            var employees = db.Query<EmployeeResponse>(query, new {page, rows});
            //{page, rows} -> parametros que estao vindo ali da consulta  "OFFSET (@page - 1) * @rows FETCH NEXT @rows ROWS ONLY" TEM QUE TER O NOME IGUAL AMBOS
            //@ -> O @rows - @page diz que esses caras sao parametros, ou seja eu colocar um @ antes do nome eu torno ela uma variavel pra passar por parametro

            return Results.Ok(employees);
        }
        */
        //--------- CÒDIGO NOVO COM O DAPPER  ---------  
        #endregion


        #region
        //--------- CÒDIGO NOVO COM O DAPPER REFATORADO  ---------        
        public static IResult Action(int? page, int? rows, QueryAllUsersWithClaimName query)
        {     
            return Results.Ok(query.Execute(page.Value, rows.Value)); //Value -> pois o page e rows Aqui sao nullable e na classe QueryAllUsersWithClaimName não são
        }
        //--------- CÒDIGO NOVO COM O DAPPER REFATORADO  ---------         
        #endregion
    }
}
