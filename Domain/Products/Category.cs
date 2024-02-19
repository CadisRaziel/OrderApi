namespace OrderApi.Domain.Products
{
    public class Category: Entity //Herança (como temos varios atributos iguais, foi melhor criar uma herança)
    {          
        public string Name { get; set; }
        public bool Active { get; set; } = true; 
    }
}
