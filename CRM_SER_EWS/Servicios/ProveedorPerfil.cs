using Microsoft.EntityFrameworkCore;

namespace CRM_EWS.Servicios
{
    public sealed class ProveedorPerfil
    {
        private static class Empresas
        {
            public const string Eagle = "EAGLE";
            public const string Lincoln = "LINCOLN";
            public const string Plussm = "PLUSSM";
            public const string DelCampo = "DELCAMPO";

            public static readonly string[] Todas = { Eagle, Lincoln, Plussm, DelCampo };
        }

        private class UsuarioVendedor
        {
            public string usuario { get; set; }
            public int empleado { get; set; }
            public string empresa { get; set; }
            public string sucursal { get; set; }
            public int esGerente { get; set; }
            public int esCoordinador { get; set; }

            public UsuarioVendedor(string usuario, int empleado, string empresa, string sucursal, int esGerente, int esCoordinador)
            {
                this.usuario = usuario;
                this.empleado = empleado;
                this.empresa = empresa;
                this.sucursal = sucursal;
                this.esGerente = esGerente;
                this.esCoordinador = esCoordinador;
            }
        }

        private class UsuarioEmpresaSucursal
        {
            public string usuario { get; set; }
            public string sucursal { get; set; }
            public string empresa { get; set; }
            public string cveTipo { get; set; }
        }

        private class UsuarioGobPermiso
        {
            public string usuario { get; set; }
            public string tipoPermiso { get; set; }
            public string permiso { get; set; }
            public string estatus { get; set; }

            public UsuarioGobPermiso(string usuario, string tipoPermiso)
            {
                this.usuario = usuario;
                this.tipoPermiso = tipoPermiso;
                this.permiso = "SI";
                this.estatus = "ACTIVO";
            }

            public UsuarioGobPermiso()
            {
                this.usuario = String.Empty;
                this.tipoPermiso = String.Empty;
                this.permiso = String.Empty;
                this.estatus = String.Empty;
            }
        }

        private class AutorizacionesContext : DbContext
        {
            private readonly IConfiguration configuration;
            public DbSet<UsuarioEmpresaSucursal> usuarioEmpresaSucursales { get; set; }
            public DbSet<UsuarioVendedor> usuarioVendedor { get; set; }
            public DbSet<UsuarioGobPermiso> permisos { get; set; }

            public AutorizacionesContext(IConfiguration configuration)
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
                modelBuilder.Entity<UsuarioEmpresaSucursal>().ToView("UsuarioEmpresaSucursal", schema: "ews");
                modelBuilder.Entity<UsuarioEmpresaSucursal>().HasNoKey();

                modelBuilder.Entity<UsuarioVendedor>().ToView("UsuarioVendedor", schema: "ews");
                modelBuilder.Entity<UsuarioVendedor>().HasNoKey();

                modelBuilder.Entity<UsuarioGobPermiso>().ToTable("Gob_UsuariosPermisos", schema: "dbo");
                modelBuilder.Entity<UsuarioGobPermiso>().Property(u => u.usuario).HasColumnName("Id_Usuario");
                modelBuilder.Entity<UsuarioGobPermiso>().Property(u => u.tipoPermiso).HasColumnName("TipoPermiso");
                modelBuilder.Entity<UsuarioGobPermiso>().Property(u => u.permiso).HasColumnName("Permiso");
                modelBuilder.Entity<UsuarioGobPermiso>().Property(u => u.estatus).HasColumnName("Estatus");
                modelBuilder.Entity<UsuarioGobPermiso>().HasNoKey();
            }
        }

        private const string IdEmpresa = "EmpresaId";

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;

        public ProveedorPerfil(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
        }

        public string Usuario
        {
            get
            {
                return Utilerias.GetUserName(this.httpContextAccessor.HttpContext.Request);
            }
        }

        public string GetEmpresa()
        {
            var empresaHeader = httpContextAccessor.HttpContext?.Request.Headers[IdEmpresa];
            if (empresaHeader is null || !empresaHeader.HasValue || !Empresas.Todas.Contains(empresaHeader.Value.ToString().ToUpper()))
            {
                throw new ApplicationException("El encabezado empresa no está presente o no es válido");
            }
            return empresaHeader.Value.ToString().ToUpper();
        }

        public List<String> GetSucursales()
        {
            var nombreUsuario = Utilerias.GetUserName(this.httpContextAccessor.HttpContext.Request);
            var empresa = this.GetEmpresa();
            using (var context = new AutorizacionesContext(configuration))
            {
                var relacionUES = context.usuarioEmpresaSucursales.Where(u => u.usuario.Equals(nombreUsuario) && u.empresa.Equals(empresa)).ToList<UsuarioEmpresaSucursal>();
                if (relacionUES.Count == 0)
                {
                    throw new ApplicationException("El usuario no tiene acceso a ninguna sucursal");
                }
                return relacionUES.Select(r => r.sucursal).ToList<String>();
            }
        }

        public bool EsGerenteVentas()
        {
            var nombreUsuario = Utilerias.GetUserName(this.httpContextAccessor.HttpContext.Request);
            var empresa = this.GetEmpresa();
            using (var context = new AutorizacionesContext(configuration))
            {
                var datosVendedor = context.usuarioVendedor.Where(uv => uv.usuario.Equals(nombreUsuario) && uv.empresa.Equals(empresa)).FirstOrDefault();
                if (datosVendedor is null) { datosVendedor = new UsuarioVendedor(nombreUsuario, -1, empresa, String.Empty, 1, 1); }
                return datosVendedor.esGerente > 0 || datosVendedor.esCoordinador > 0;
            }
        }

        public bool TienePermiso(string usuario, string permiso)
        {
            var ugp = new UsuarioGobPermiso(usuario, permiso);
            using (var context = new AutorizacionesContext(configuration))
            {
                return context.permisos.Where(p => p.usuario.Equals(usuario) && p.tipoPermiso.Equals(permiso) && p.estatus.Equals(ugp.estatus) && p.permiso.Equals(ugp.permiso)).FirstOrDefault() != null;
            }
        }
    }
}
