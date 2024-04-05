using CRM_EWS.CRM.Helpers;

namespace CRM_EWS.CRM.Models.Tareas
{
    public class RegistroTareaVista
    {
        public int idTarea { get; set; }
        public int idTipoTarea { get; set; }
        public string descripcionTipoTarea { get; set; }
        public string colorTipoTarea { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public string descripcion { get; set; }
        public string notas { get; set; }
        public int idEmpleado { get; set; }
        public int idEstado { get; set; }
        public string descripcionEstado { get; set; }
        public string colorEstado { get; set; }
        public string idCliente { get; set; }
        public string nombreCliente { get; set; }
    }

    public class RegistroTareaVistaQuery : IQuery
    {
        public List<int> tiposTarea { get; set; }
        public List<int> empleados { get; set; }
        public List<int> estadoTarea { get; set; }
        public DateTime fechaEspecifica { get; set; }
        public bool rangoFechas { get; set; }
        public DateTime fechaInicioBusqueda { get; set; }
        public DateTime fechaFinBusqueda { get; set; }
        private int _page;
        public int Page { get => _page; set => _page = value; }

        private int _pageSize;
        public int PageSize { get => _pageSize; set => _pageSize = value; }

        private string _sortBy;
        public string SortBy { get => _sortBy; set => _sortBy = value; }

        private bool _isSortAscending;
        public bool IsSortAscending { get => _isSortAscending; set => _isSortAscending = value; }

        public RegistroTareaVistaQuery()
        {
            tiposTarea = new List<int>();
            empleados = new List<int>();
            estadoTarea = new List<int>();
            fechaEspecifica = DateTime.Now;
            rangoFechas = false;
            fechaInicioBusqueda = DateTime.Now;
            fechaFinBusqueda = DateTime.Now;
            Page = 1;
            PageSize = 20;
            SortBy = "fechaFin";
            IsSortAscending = false;
        }
    }
}
