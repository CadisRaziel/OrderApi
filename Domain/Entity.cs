using Flunt.Notifications;

namespace OrderApi.Domain
{
    public abstract class Entity : Notifiable<Notification> //Utilizando o flunt nós iremos trabalhar com notificação, ou seja essa classe vai enviar mensagens para outra classe pra realizar as validações
    {
         
        public Entity()
        {
            //Toda vez que eu instanciar tanto category quanto product eu ja vou ter o id pronto!! 
            Id = Guid.NewGuid();          
        }

        //Guid -> Nos que geramos os numeros (ou seja ao inves do .net criar sequenciado é nós que criamos)
        //Caso fosse um inteiro a gente iria precisa do objeto salvo no banco de dados
        //Ja o guid é feito em tempo de execução e antes de armazenar no banco eu ja tenho a referencia dele (ja tenho ele gerado)
        //Ele é parecido com um hash (uuid)
        //Ele nao é tão performatico como um INT
        public Guid Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string EditedBy { get; set; }
        public DateTime EditedOn { get; set; } 
    }
}
