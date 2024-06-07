using EWS_ContextoGobernador.Contexto;
using Microsoft.EntityFrameworkCore;

namespace CRM_EWS.CRM.Models.Equipos
{
    public class EquipoContext : GobernadorContext
    {
        private readonly IConfiguration configuration;

        public DbSet<EquipoEntity> equipos { get; set; }
        public DbSet<EquipoPrestamo> prestamos { get; set; }
        public DbSet<EquipoTraslado> traslados { get; set; }

        public EquipoContext(IConfiguration configuration) : base(configuration)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EquipoEntity>().ToTable("Inv_MaquinariaEquipo", schema: "acf");
            modelBuilder.Entity<EquipoEntity>().HasKey(e => e.idActf);
            modelBuilder.Entity<EquipoEntity>().Property(e => e.idActf).HasColumnName("ID_ActF");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.nombreEquipo).HasColumnName("Nombre/Descripcion");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.numSerie).HasColumnName("Num_Serie");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.costoCompra).HasColumnName("CostoCompra");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.fechaCompra).HasColumnName("Fecha_adquisicion");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.modelo).HasColumnName("Modelo");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.marca).HasColumnName("Marca");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.sucursalCompra).HasColumnName("Sucursal");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.sucursalActual).HasColumnName("sucursalActual");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.idCategoria).HasColumnName("Id_Categoria");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.inicioOperacion).HasColumnName("IniOperacion");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.usuarioAlta).HasColumnName("Usuario_Alta");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.fechaAlta).HasColumnName("Fecha_Alta");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.fechaFum).HasColumnName("Fecha_Fum");
            modelBuilder.Entity<EquipoEntity>().Property(e => e.usuarioFum).HasColumnName("Usuario_Fum");

            modelBuilder.Entity<EquipoPrestamo>().ToTable("Inv_MaquinariaEquipoPrestamo", schema: "acf");
            modelBuilder.Entity<EquipoPrestamo>().Ignore(ep => ep.nombreCliente);
            modelBuilder.Entity<EquipoPrestamo>().HasKey(ep => ep.idPrestamo);

            modelBuilder.Entity<EquipoTraslado>().ToTable("Inv_MaquinariaEquipoTraslado", schema: "acf");
            modelBuilder.Entity<EquipoTraslado>().HasKey(ep => ep.idTraslado);
        }
    }
}
