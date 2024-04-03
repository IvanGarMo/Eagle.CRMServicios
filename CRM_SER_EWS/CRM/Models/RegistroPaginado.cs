namespace CRM_SER_EWS.CRM.Models
{
    public class ResultadoPaginado<T>
    {
        public List<T> items { get; set; }
        public int numeroPagina { get; set; }
        public int totalPaginas { get; set; }
        public int totalResultados { get; set; }
        public int tamanoPagina { get; set; }

        public ResultadoPaginado(List<T> items, int numeroPagina, int totalResultados, int tamanoPagina)
        {
            this.items = items;
            this.totalPaginas = (int)Math.Ceiling(totalResultados / (decimal)tamanoPagina);
            this.numeroPagina = numeroPagina;
            this.totalResultados = totalResultados;
            this.tamanoPagina = tamanoPagina;
        }
    }
}
