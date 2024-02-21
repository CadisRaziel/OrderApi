namespace OrderApi.Endpoints.Employees
{
    public record  EmployeeResponse(string email, string name);
    //Lembrar de voltar o string id, eu removi ele pra fazer o dapper, porem com paginação sem o dapper eu posso voltar ele
    //é que no dapper eu nao fiz a consulta 
}
