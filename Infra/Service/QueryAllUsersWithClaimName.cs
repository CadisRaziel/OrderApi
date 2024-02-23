using Dapper;
using Microsoft.Data.SqlClient;
using OrderApi.Endpoints.Employees;

namespace OrderApi.Infra.Service
{
    public class QueryAllUsersWithClaimName
    {
        private readonly IConfiguration configuration;
        public QueryAllUsersWithClaimName(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<EmployeeResponse>> Execute(int page, int rows)
        {
            var db = new SqlConnection(configuration["ConnectionStrings:OrderApiDb"]);

            var query =
                @"  SELECT Email, ClaimValue as Name
                    FROM AspNetUsers apelidoUser INNER JOIN AspNetUserClaims apelidoClaims
                    on apelidoUser.Id = apelidoClaims.UserId and ClaimType = 'Name'
                    ORDER BY Name
                    OFFSET (@page - 1) * @rows ROWS FETCH NEXT @rows ROWS ONLY
                 ";

            return await db.QueryAsync<EmployeeResponse>(query, new { page, rows });
        }
    }
}
