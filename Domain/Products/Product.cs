using Flunt.Validations;
using System.Diagnostics.Contracts;

namespace OrderApi.Domain.Products
{
    public class Product : Entity //Herança (como temos varios atributos iguais, foi melhor criar uma herança)
    {          
        public string Name { get; private set; }
        public Guid CategoryId { get; private set; } //-> Chave estrangeira do Category (no banco ele vai criar com esse nome)
        public Category Category { get; private set; }
        public string Description { get; private set; }
        public bool HasStock { get; private set; }
        public bool Active { get; private set; } = true;
        public decimal Price { get; private set; }

        private Product() { } //-> Lembre-se sempre de criar um construtor vazio(se nao vai dar erro em ouros endpoints)

        public Product(string name, Category category, string description, bool hasStock, decimal price, string createdBy)
        {
            Name = name;
            Category = category;
            Description = description;
            HasStock = hasStock;     
            Price = price;

            CreatedBy = createdBy;
            EditedBy = createdBy;
            CreatedOn = DateTime.Now;
            EditedOn = DateTime.Now;

            Validate();

        }

        private void Validate()
        {
            var contract = new Contract<Product>()
                .IsNotNullOrEmpty(Name, "Name")
                .IsGreaterOrEqualsThan(Name, 3, "Name")
                .IsNotNull(Category, "Category", "Category not found")
                .IsNotNullOrEmpty(Description, "Description")
                .IsGreaterOrEqualsThan(Price, 1, "price")
                .IsGreaterOrEqualsThan(Description, 3, "Description")                
                .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
                .IsNotNullOrEmpty(EditedBy, "EditedBy");
            AddNotifications(contract);
        }

    }    
}
