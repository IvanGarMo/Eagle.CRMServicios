using CRM_EWS.CRM.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM_EWS.CRM.Helpers
{
    public class RegistroAceiteContext : DbContext
    {
        public DbSet<RegistroAnalisisAceiteEntity> analisis { get; set; }
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
            modelBuilder.Entity<RegistroAnalisisAceiteEntity>().HasKey(r => r.idRegistro);
            modelBuilder.Entity<RegistroAnalisisAceiteEntity>().ToTable("AnalisisAceite", schema: "serv");
            modelBuilder.Entity<RegistroAnalisisAceiteEntity>().Property(r => r.idRegistro).ValueGeneratedOnAdd();
            modelBuilder.Entity<RegistroAnalisisAceiteEntity>().HasQueryFilter(r => r.activo);
        }
    }
}
