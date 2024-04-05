using System.ComponentModel.DataAnnotations;

namespace CRM_EWS.Analisis
{
    public class AnalisisAceite
    {
        public int idAnalisis { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe especificar un nombre del análisis")]
        [StringLength(100, ErrorMessage = "El campo nombre no puede exceder de 100 caracteres")]
        public string nombre { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe escribir una descripción del estudio")]
        [StringLength(100, ErrorMessage = "El campo nombre no puede exceder de 500 caracteres")]
        public string descripcion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe seleccionar una moneda")]
        [StringLength(100, ErrorMessage = "El campo moneda no puede exceder de 3 caracteres")]
        public string moneda { get; set; }

        [Required]
        [Range(0, 10000.0, ErrorMessage ="El costo no es válido")]
        public decimal costo { get; set; }
        public string? usuario { get; set; }
        public bool activo { get; set; }

        public AnalisisAceite()
        {
        }

        public AnalisisAceite(int idAnalisis, string nombre, string descripcion, string moneda, decimal costo, string? usuario, bool activo)
        {
            this.idAnalisis = idAnalisis;
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.moneda = moneda;
            this.costo = costo;
            this.usuario = usuario;
            this.activo = activo;
        }

        public AnalisisAceite(int idAnalisis, string nombre, string descripcion, string moneda, decimal costo)
        {
            this.idAnalisis = idAnalisis;
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.moneda = moneda;
            this.costo = costo;
            this.activo = true;
            this.usuario = String.Empty;
        }
    }
}
