using CRM_EWS.CRM.Models;
using CRM_EWS.CRM.Models.Capacitaciones;
using Microsoft.EntityFrameworkCore;

namespace CRM_EWS.CRM.Helpers
{
    public class RegistroCapacitacionContext : DbContext
    {
        private readonly IConfiguration configuration;
        public DbSet<Capacitacion> capacitaciones { get; set; }
        public DbSet<Empleado> empleados { get; set; }
        public DbSet<CapacitacionEmpleado> capacitadores { get; set; }
        public DbSet<CapacitacionSubmodulo> submodulos { get; set; }
        public RegistroCapacitacionContext(IConfiguration configuration)
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

            //Configuro la tabla de capacitaciones
            modelBuilder.Entity<Capacitacion>().ToTable("Capacitaciones", schema: "serv");
            modelBuilder.Entity<Capacitacion>().HasKey(c => c.idCapacitacion);
            modelBuilder.Entity<Capacitacion>().Ignore(c => c.asistentes);
            modelBuilder.Entity<Capacitacion>().Property(c => c.asistentesSerialized).HasColumnName("participantes");
            modelBuilder.Entity<Capacitacion>().Property(c => c.incluyeCertificado).HasColumnName("clienteDeseaCertificado");
            modelBuilder.Entity<Capacitacion>().Ignore(c => c.Submodulos);
            modelBuilder.Entity<Capacitacion>().Ignore(c => c.instructores);
            modelBuilder.Entity<Capacitacion>().Ignore(c => c.modulos);

            //Configuro la tabla de submodulos
            modelBuilder.Entity<CapacitacionSubmodulo>().ToTable("CapacitacionesSubmodulos", schema: "serv");
            modelBuilder.Entity<CapacitacionSubmodulo>().HasKey(cs => new { cs.idCapacitacion, cs.idSubmodulo });

            //Configuro la tabla de instructores
            modelBuilder.Entity<CapacitacionEmpleado>().ToTable("CapacitacionesInstructores", schema: "serv");
            modelBuilder.Entity<CapacitacionEmpleado>().HasKey(ce => new { ce.idCapacitacion, ce.idEmpleado });

            //Configuro la tabla de empleados
            modelBuilder.Entity<Empleado>().ToTable("Nom_Empleados", schema: "dbo");
            modelBuilder.Entity<Empleado>().HasKey(e => e.idEmpleado);
            modelBuilder.Entity<Empleado>().Property(e => e.idEmpleado).HasColumnName("Id_Empleado");
            modelBuilder.Entity<Empleado>().Property(e => e.nombre).HasColumnName("Nombre");
        }
    }
}
