using Newtonsoft.Json;

namespace CRM_EWS.CRM.Models.Capacitaciones
{
    public class Capacitacion
    {
        public int idCapacitacion { get; set; }
        public string idCliente { get; set; }
        public int idCurso { get; set; }
        public string sucursal { get; set; }
        public List<CapacitacionAsistente> asistentes { get; set; }
        public List<CapacitacionSubmodulo> Submodulos { get; set; }
        public List<CapacitacionModulo> modulos { get; set; }
        public string asistentesSerialized { 
            get 
            {
                return JsonConvert.SerializeObject(asistentes);
            }
            set 
            { 
                asistentes = String.IsNullOrWhiteSpace(value) ? new List<CapacitacionAsistente>() : JsonConvert.DeserializeObject<List<CapacitacionAsistente>>(value);
            } 
        }
        public bool incluyeCertificado { get; set; }
        public string descripcion { get; set; }
        public List<CapacitacionEmpleado> instructores { get; set; }
    }

    public class CapacitacionAsistente
    {
        public string nombre { get; set; }
        public string apPaterno { get; set; }
        public string apMaterno { get; set; }
    }

    public class CapacitacionEmpleado
    {
        public int idCapacitacion { get; set; }
        public int idEmpleado { get; set; }
        public string nombre { get; set; }
        public bool activo { get; set; }

        public CapacitacionEmpleado(int idCapacitacion, int idEmpleado, bool activo)
        {
            this.idCapacitacion = idCapacitacion;
            this.idEmpleado = idEmpleado;
            this.activo = activo;
            this.nombre = String.Empty;
        }

        public CapacitacionEmpleado(int idCapacitacion, int idEmpleado, string nombre, bool activo)
        {
            this.idCapacitacion = idCapacitacion;
            this.idEmpleado = idEmpleado;
            this.nombre = nombre;
            this.activo = activo;
        }
    }

    public class CapacitacionSubmodulo
    {
        public int idCapacitacion { get; set; }
        public string nombreSubmodulo { get; set; }
        public int idSubmodulo { get; set; }
        public bool activo { get; set; }

        public CapacitacionSubmodulo(int idCapacitacion, int idSubmodulo, bool activo)
        {
            this.idCapacitacion = idCapacitacion;
            this.idSubmodulo = idSubmodulo;
            this.activo = activo;
            this.nombreSubmodulo = String.Empty;
        }

        public CapacitacionSubmodulo()
        {
            this.nombreSubmodulo = String.Empty;
        }
    }

    public class CapacitacionModulo
    {
        public int idModulo { get; set; }
        public string nombre { get; set; }
        public List<CapacitacionSubmodulo> submodulos { get; set; }
    }
}
