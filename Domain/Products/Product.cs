namespace OrderApi.Domain.Products
{
    public class Product : Entity //Herança (como temos varios atributos iguais, foi melhor criar uma herança)
    {          
        public string Name { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public bool HasStock { get; set; } 
    }
}
