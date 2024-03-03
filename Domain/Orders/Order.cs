using Flunt.Validations;
using OrderApi.Domain.Products;

namespace OrderApi.Domain.Orders
{
    public class Order : Entity
    {
        public string ClientId { get; private set; }
        public List<Product> Products { get; private set; }
        public decimal Total { get; private set; }
        public string DeliveryAddress { get; private set; }

        private Order() { }

        public Order(string clientId, string clientName, List<Product> products, string deliveryAddress)
        {
            ClientId = clientId;
            Products = products;
            DeliveryAddress = deliveryAddress;
            CreatedBy = clientName; //-> Por quem foi criado
            EditedBy = clientName; //-> Por quem foi editado
            CreatedOn = DateTime.UtcNow;
            EditedOn = DateTime.UtcNow;

            Total = 0;
            foreach (var item in Products)
            {
                //Percoro a lista de produtos pra eu somar atras do preco
                //eu poderia fazer isso no endpoint na hora de criar o produto, mas é sempre bom fazer isso na entidade
                Total += item.Price;
            }

            Validate();
        }

        private void Validate()
        {
            var contract = new Contract<Order>()
                .IsNotNull(ClientId, "Client")
                .IsTrue(Products != null && Products.Any(), "Products")
                .IsNotNullOrEmpty(DeliveryAddress, "DeliveryAddress");
            AddNotifications(contract);
        }
    }
}
