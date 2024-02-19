using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Products;

namespace OrderApi.Infra.Data
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Abaixo colocamos todas as strings como 100 caracteres porém aqui eu digo(respeite esses valores acima da função debaixo), ou seja essa função tem mais prioridade
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(255);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
            //Função que altera algumas convenções do EF Core
        {
            configuration.Properties<string>().HaveMaxLength(100); //-> Configuração para que todas as propriedades do tipo string tenham no máximo 100 caracteres
        }

    }
}
