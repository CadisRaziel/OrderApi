namespace OrderApi.Endpoints.Clients
{
    public record ClientsRequest(string Email, string Password, string Name, string Cpf);
    
}
