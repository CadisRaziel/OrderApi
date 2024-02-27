using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Products;

namespace OrderApi.Infra.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser> //-> Iremos utilizar o IdentityUser para autenticação (repare que ele é nativo e é uma combinação com o DbContext pois ele precisa ir nas tabelas do db
    {
        /*
         IdentityDbContext<IdentityUser> -> adicionado isso ganhamos o seguinte
            - Gerenciamento de usuario
            - Gerenciamento de atributos (claims)
            - Gerenciamento de password/usuario(login)
            - Gerenciamento de roles
            - Gerenciamento de autenticação
            - Gerenciamento de autorização
         */

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configurando o Identity para nao der erro de primareKey na hora de gera migration
            base.OnModelCreating(modelBuilder); //base. -> estou chamando o OnModelCreating da classe pai que é o IdentityDbContext

            //Abaixo colocamos todas as strings como 100 caracteres porém aqui eu digo(respeite esses valores acima da função debaixo), ou seja essa função tem mais prioridade
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(255);  
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(10,2)").IsRequired(); //-> 10 digitos antes da virgula e 2 casas decimais depois da virgula
            //Detalhe do Price e da coluna decimal, a partir do momento que eu crio uma coluna "Obrigatoria(isRequired)" antes de fazer migration eu preciso deletar tudo que tem nessa tabela de product
            

            modelBuilder.Ignore<Notification>(); //-> Ignorando a classe de notificação do flunt para parar de dar erro de chave primaria
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
            //Função que altera algumas convenções do EF Core
        {
            configuration.Properties<string>().HaveMaxLength(100); //-> Configuração para que todas as propriedades do tipo string tenham no máximo 100 caracteres
        }

    }
}
