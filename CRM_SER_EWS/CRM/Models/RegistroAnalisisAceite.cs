using CRM_EWS.CRM.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CRM_EWS.CRM.Models
{
    public abstract class RegistroAnalisisAceiteBase
    {
        public int idRegistro { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El id del cliente es un campo requerido")]
        [MaxLength(10, ErrorMessage = "El campo id del cliente no puede exceder de 10 caracteres")]
        public string idCliente { get; set; }
        [Required(ErrorMessage = "Fecha del análisis es un campo requerido")]
        public DateTime fecha { get; set; }
        [Required(ErrorMessage = "El campo tipo de analisis es necesario")]
        public int tipoAnalisis { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo id del vendedor es requerido")]
        [MaxLength(10, ErrorMessage = "El campo id del vendedor no puede exceder de 10 caracteres")]
        public string idVendedor { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo sucursal es requerido")]
        [MaxLength(30, ErrorMessage = "El campo sucursal no puede exceder de 30 caracteres")]
        public string sucursal { get; set; }
        [MaxLength(100, ErrorMessage = "El campo id de equipo no puede exceder de 100 caracteres")]
        public string idEquipo { get; set; }
        [MaxLength(200, ErrorMessage = "El campo compartimiento no puede exceder de 200 caracteres")]
        public string compartimiento { get; set; }
        [MaxLength(200, ErrorMessage ="El campo tipo de lubricante no puede exceder de 200 caracteres")]
        public string tipoLubricante { get; set; }
        [MaxLength(100, ErrorMessage = "El campo odometro total no puede exceder de 100 caracteres")]
        public string odometroTotal { get; set; }
        [MaxLength(100, ErrorMessage = "El campo odometro aceite no puede exceder de 100 caracteres")]
        public string odometroAceite { get; set; }

        [MaxLength(70, ErrorMessage = "El campo número de guía no puede exceder de 70 caracteres")]
        public string numeroGuia { get; set; }

        protected RegistroAnalisisAceiteBase()
        {
            idRegistro = 0;
            tipoAnalisis = -1;
            sucursal = String.Empty;
            idCliente = String.Empty;
            compartimiento = String.Empty;
            tipoLubricante = String.Empty;
            odometroTotal = String.Empty;
            odometroAceite = String.Empty;
            idVendedor = String.Empty;
            fecha = DateTime.Now;
            idEquipo = String.Empty;
            numeroGuia = String.Empty;
        }
    }

    public class RegistroAnalisisAceite : RegistroAnalisisAceiteBase
    {
        public RegistroAnalisisAceite() : base() { }
    }

    public class RegistroAnalisisAceiteEntity : RegistroAnalisisAceiteBase
    {
        public decimal costo { get; set; }
        public string moneda { get; set; }
        public bool activo { get; set; }
        public string usuario { get; set; }

        public RegistroAnalisisAceiteEntity() : base()
        {
            costo = 0.0M;
            activo = true;
            usuario = String.Empty;
        }
    }

    public class RegistroAnalisisAceiteViewModel : RegistroAnalisisAceite
    {
        public string nombreCliente { get; set; }
        public string nombreVendedor { get; set; }
        public string descripcionTipoAnalisis { get; set; }
        public decimal costo { get; set; }
        public string moneda { get; set; }
        public RegistroAnalisisAceiteViewModel() : base()
        {
            nombreCliente = String.Empty;
            nombreVendedor = String.Empty;
            descripcionTipoAnalisis = String.Empty;
        }
    }

    public class QueryRegistroAnalisisAceite : IQuery
    {
        public string idCliente { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public int tipoAnalisis { get; set; }

        private int _page;
        public int Page { get => _page; set => _page = value; }

        private int _pageSize;
        public int PageSize { get => _pageSize; set => _pageSize = value; }

        private string _sortBy;
        public string SortBy { get => _sortBy; set => _sortBy = value; }

        private bool _isSortAscending;
        public bool IsSortAscending { get => _isSortAscending; set => _isSortAscending = value; }

        public QueryRegistroAnalisisAceite()
        {
            idCliente = String.Empty;
            fechaInicio = new DateTime(DateTime.Now.Year, 1, 1);
            fechaFin = DateTime.Now;
            tipoAnalisis = 0;
            Page = 1;
            PageSize = 20;
            _sortBy = SortBy = String.Empty;
            IsSortAscending = false;
        }
    }
}
