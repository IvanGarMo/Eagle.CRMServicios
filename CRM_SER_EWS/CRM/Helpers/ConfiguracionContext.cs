﻿using CRM_SER_EWS.Analisis;
using CRM_SER_EWS.CRM.Models;
using CRM_SER_EWS.Servicios;
using Microsoft.EntityFrameworkCore;

namespace CRM_SER_EWS.CRM.Helpers
{
    public class ConfiguracionContext : DbContext
    {
        public DbSet<Tarea> tareas { get; set; }
        public DbSet<AnalisisAceite> analisis {get;set;}

        public DbSet<Curso> cursos { get; set; }
        public DbSet<CursoModulo> modulosCurso { get; set; }
        public DbSet<CursoSubmodulo> submodulosCurso { get; set; }

        private readonly IConfiguration _configuration;

        public ConfiguracionContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Configuración de la tabla de tareas
            modelBuilder.Entity<Tarea>().HasKey(t => t.idTarea);
            modelBuilder.Entity<Tarea>().ToTable("CatTareas", schema: "serv");
            modelBuilder.Entity<Tarea>().Property(t => t.idTarea).ValueGeneratedOnAdd();

            //Configuración de la tabla de analisis de haceite
            modelBuilder.Entity<AnalisisAceite>().HasKey(a => a.idAnalisis);
            modelBuilder.Entity<AnalisisAceite>().ToTable("CatAnalisisAceite", schema: "serv");
            modelBuilder.Entity<AnalisisAceite>().Property(a => a.idAnalisis).ValueGeneratedOnAdd();

            //Configuración de las tablas de cursos
            //Curso
            modelBuilder.Entity<Curso>().HasKey(c => c.idCurso);
            modelBuilder.Entity<Curso>().ToTable("CatCursos", schema: "serv");
            modelBuilder.Entity<Curso>().Property(c => c.idCurso).ValueGeneratedOnAdd();
            modelBuilder.Entity<Curso>().HasMany(c => c.Modulos).WithOne(c => c.curso).HasForeignKey(m => m.idCurso).IsRequired();
            //Modulos
            modelBuilder.Entity<CursoModulo>().HasKey(cm => cm.idModulo);
            modelBuilder.Entity<CursoModulo>().ToTable("CatCursosModulos", schema: "serv");
            modelBuilder.Entity<CursoModulo>().Property(cm => cm.idModulo).ValueGeneratedOnAdd();
            modelBuilder.Entity<CursoModulo>().HasMany(cm => cm.Submodulos).WithOne(csm => csm.modulo).HasForeignKey(csm => csm.idModulo).IsRequired();
            //Submodulos
            modelBuilder.Entity<CursoSubmodulo>().HasKey(csm => csm.idSubmodulo);
            modelBuilder.Entity<CursoSubmodulo>().ToTable("CatCursosSubmodulos", schema: "serv");
            modelBuilder.Entity<CursoSubmodulo>().Property(csm => csm.idSubmodulo).ValueGeneratedOnAdd();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("connectionString"));
        }
    }
}