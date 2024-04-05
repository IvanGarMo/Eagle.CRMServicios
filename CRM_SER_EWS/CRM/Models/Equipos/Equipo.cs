using CRM_EWS.CRM.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_EWS.CRM.Models.Equipos
{
    public class EquipoIndice
    {
        public int idActF { get; set; }
        public string nombreEquipo { get; set; }
        public string idZona { get; set; }
        public string descripcionZona { get; set; }
        public string sucursal { get; set; }
        public bool asignado { get; set; }
    }
    
    public class Equipo
    {
        public int idActf { get; set; }
        public string nombreEquipo { get; set; }
        public string numSerie { get; set; }
        public decimal costoCompra { get; set; }
        public DateTime fechaCompra { get; set; }
        public string modelo { get; set; }
        public string marca { get; set; }
        public string sucursalCompra { get; set; }
        public string sucursalActual { get; set; }
        public bool asignado { get; set; }
    }

    public class EquipoPrestamo
    {
        public string idPrestamo { get; set; }
        public int idActF { get; set; }
        public string idCliente { get; set; }
        public string nombreCliente { get; set; }
        public DateTime fechaInicio { get; set; }
        public int idTareaInstalacion { get; set; }
        public bool prestamoActivo { get; set; }
        public DateTime? fechaFin { get; set; }
        public int? idTareaRetiro { get; set; }


    }

    
    public class EquipoTraslado
    {
        public string idTraslado { get; set; }
        public int idActF { get; set; }
        public string sucursalOrigen { get; set; }
        public DateTime fechaSalida { get; set; }
        public string? sucursalDestino { get; set; }
        public DateTime? fechaDestino { get; set; }
    }

    public class QueryEquipos : IQuery
    {
        private int _page;
        public int Page { get => _page; set => _page = value; }

        private int _pageSize;
        public int PageSize { get => _pageSize; set => _pageSize = value; }

        private string _sortBy;
        public string SortBy { get => _sortBy; set => _sortBy = value; }

        private bool _isSortAscending;
        public bool IsSortAscending { get => _isSortAscending; set => _isSortAscending = value; }
        public string Region { get; set; }
        public List<string> zona { get; set; }
        public List<string> Sucursal { get; set; }
        public int Estatus { get; set; }

        public QueryEquipos()
        {
            Page = 1;
            PageSize = 20;
            SortBy = String.Empty;
            _sortBy = SortBy;
            IsSortAscending = false;
            zona = new List<String>();
            Region = String.Empty;
            Sucursal = new List<string>();
            Estatus = 0;
        }
    }

    public class EquipoMovimiento
    {
        public int idActf { get; set; }
        public bool esInstalacion { get; set; }
        [SeleccionarInstalacionRetiro]
        public bool esRetiro { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string idCliente { get; set; }
        [Required]
        [Range(minimum: 1, maximum: Int64.MaxValue)]
        public int domicilioInstalacion { get; set; }
        [Required]
        public DateTime fechaInstalacionRetiro { get; set; }
        [SeleccionarEmpleados]
        public List<int> empleados { get; set; }
        public int idTipoTarea { get; set; }

        public EquipoMovimiento()
        {
            idActf = 0;
            esInstalacion = esRetiro = false;
            idCliente = String.Empty;
            domicilioInstalacion = -1;
            fechaInstalacionRetiro = DateTime.Now;
            empleados = new List<int>();
        }
    }

    public class SeleccionarEmpleados : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            EquipoMovimiento em = (EquipoMovimiento)validationContext.ObjectInstance;
            if(em.empleados.Count == 0)
            {
                return new ValidationResult("Se debe especificar que empleados proveerán el servicio");
            }
            return ValidationResult.Success;
        }
    }

    public class SeleccionarInstalacionRetiro : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            EquipoMovimiento em = (EquipoMovimiento)validationContext.ObjectInstance;
            if (em.esRetiro == em.esInstalacion)
            {
                return new ValidationResult("Se debe especificar si es instalación o retiro");
            }
            return ValidationResult.Success;
        }
    }
}
