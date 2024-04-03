using CRM_SER_EWS.CRM.Helpers;

namespace CRM_SER_EWS.Servicios
{
    public class Curso
    {
        public int idCurso { get; set; }
        public string nombre { get; set; }
        public bool cursoChevron { get; set; }
        public string? usuario { get; set; }
        public DateTime fechaCreacion { get; set; }
        public bool activo { get; set; }
        public List<CursoModulo>? Modulos { get; set; }
    }

    public class CursoQuery : IQuery
    {
        public string nombre { get; set; }

        private int _page;
        public int Page { get => _page; set => _page = value; }

        private int _pageSize;
        public int PageSize { get => _pageSize; set => _pageSize = value; }

        private string _sortBy;
        public string SortBy { get => _sortBy; set => _sortBy = value; }

        private bool _isSortAscending;
        public bool IsSortAscending { get => _isSortAscending; set => _isSortAscending = value; }

        public CursoQuery()
        {
            nombre = String.Empty;
            Page = 1;
            PageSize = 20;
            SortBy = "fechaCreacion";
            IsSortAscending = true;
        }
    }
}