using OrderApi.Domain.Products;

namespace OrderApi.Endpoints.Products
{
    public record ProductResponse(Guid id, string Name, string CategoryName, string Description, decimal Price, bool HasStock, bool Active);    
}