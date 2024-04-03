using CRM_SER_EWS.CRM.Helpers;

namespace CRM_SER_EWS.CRM.Models
{
    public class RegistroAnalisisAceite
    {
        public int idRegistro { get; set; }
        public string idCliente { get; set; }
        public DateTime fecha { get; set; }
        public int tipoAnalisis { get; set; }
        public string idVendedor { get; set; }
        public string sucursal { get; set; }
        public bool activo { get; set; }
        public string? usuario { get; set; }
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
            SortBy = String.Empty;
            IsSortAscending = false;
        }
    }
}
