using CRM_SER_EWS.CRM.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM_SER_EWS.CRM.Helpers
{
    public class RegistroAceiteContext : DbContext
    {
        public DbSet<RegistroAnalisisAceite> analisis { get; set; }
        private readonly IConfiguration configuration;

        public RegistroAceiteContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("connectionString"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configuración de la tabla de analisis de aceite
            modelBuilder.Entity<RegistroAnalisisAceite>().HasKey(r => r.idRegistro);
            modelBuilder.Entity<RegistroAnalisisAceite>().ToTable("AnalisisAceite", schema: "serv");
            modelBuilder.Entity<RegistroAnalisisAceite>().Property(r => r.idRegistro).ValueGeneratedOnAdd();
        }
    }
}
