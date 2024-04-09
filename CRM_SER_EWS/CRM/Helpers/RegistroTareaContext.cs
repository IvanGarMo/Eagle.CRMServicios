using CRM_EWS.CRM.Models.Tareas;
using Microsoft.EntityFrameworkCore;

namespace CRM_EWS.CRM.Helpers
{
    public class RegistroTareaContext : DbContext
    {
        private readonly IConfiguration configuration;
        public DbSet<RegistroTareaEntity> tareas { get; set; }
        public DbSet<RelacionTareaEmpleado> relacionTareaEmpleados { get; set; }
        public DbSet<RegistroTareaVista> tareasVista { get; set; }

        public RegistroTareaContext(IConfiguration configuration)
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
            modelBuilder.Entity<RegistroTareaEntity>().ToTable("Tareas", schema: "serv");
            modelBuilder.Entity<RegistroTareaEntity>().HasKey(t => t.idTarea);
            modelBuilder.Entity<RegistroTareaEntity>().Ignore(t => t.checkList);
            modelBuilder.Entity<RegistroTareaEntity>().Ignore(t => t.empleados);
            modelBuilder.Entity<RegistroTareaEntity>().Property(t => t.checkListSerialized).HasColumnName("checklist");
            modelBuilder.Entity<RegistroTareaEntity>().Property(t => t.estado).HasColumnType("int");

            modelBuilder.Entity<RelacionTareaEmpleado>().ToTable("TareasAsignacion", schema: "serv");
            modelBuilder.Entity<RelacionTareaEmpleado>().HasKey(t => new { t.idTarea, t.idEmpleado });

            modelBuilder.Entity<RegistroTareaVista>().ToView("VistaTareas", schema: "serv");
            modelBuilder.Entity<RegistroTareaVista>().HasNoKey();
        }
    }
}
