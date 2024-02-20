using Flunt.Validations;
namespace OrderApi.Domain.Products
{
    public class Category: Entity //Herança (como temos varios atributos iguais, foi melhor criar uma herança)
    //Eu poderia tambem passar a herança de validação aqui igual esta na entity : Notifiable<Notification>, porém a entity é uma classe generica e la é melhor
    {
        public string Name { get; private set; } //private -> somente aqui dentro dessa classe podemos editar essas informações
        public bool Active { get; private set; } 

        public Category(string name, string createdBy, string editedBy)
        {

            Name = name;
            Active = true;
            CreatedBy = createdBy;
            EditedBy = editedBy;
            CreatedOn = DateTime.Now;
            EditedOn = DateTime.Now;

            //è preciso realizar a validação no construtor
            Validate();
        }

        //CTRL + R + M -> Eu seleciono todo código e faço isso ele cria uma função pra mim
        private void Validate()
        {
            var contract = new Contract<Category>()
                .IsNotNullOrEmpty(Name, "Name", "Nome é obrigatório") //-> Se eu nao colocar "Nome é obrigatório" ou qualquer outra mensagem ali, ele vai atribuir uma padrao em ingles
                .IsGreaterOrEqualsThan(Name, 3, "Name", "Nome deve ter no minimo 3 caracteres")
                .IsNotNullOrEmpty(CreatedBy, "createdBy", "createdBy error")
                .IsNotNullOrEmpty(EditedBy, "editedBy", "editedBy error");
            AddNotifications(contract);
        }

        public void EditInfo(string name, bool active)
        {
            Name = name;
            Active = active;

            //Olha que interessante ue tambem posso por um contrato de tratamento de excessão aqui.
            Validate();

        }
    }
}
