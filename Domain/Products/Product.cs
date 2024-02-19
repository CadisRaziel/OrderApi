namespace OrderApi.Domain.Products
{
    public class Product : Entity //Herança (como temos varios atributos iguais, foi melhor criar uma herança)
    {          
        public string Name { get; set; }
        public int CategoryId { get; set; } //-> Chave estrangeira do Category (no banco ele vai criar com esse nome)
        public Category Category { get; set; }
        public string Description { get; set; }
        public bool HasStock { get; set; } 
    }
}
