using System.ComponentModel.DataAnnotations;

namespace CRM_EWS.Servicios
{
    public class Tarea
    {
        public int idTarea { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe especificar un nombre de la tarea")]
        [StringLength(100, ErrorMessage = "El campo nombre no puede exceder de 100 caracteres")]
        public string nombre { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe especificar una descripción de la tarea")]
        [StringLength(300, ErrorMessage = "El campo descripción no puede exceder de 300 caracteres")]
        public string descripcion { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe especificar un color")]
        [StringLength(20, ErrorMessage = "El campo color no puede exceder de 20 caracteres")]
        public string color { get; set; }
        public string? usuario { get; set; }
        public bool activo { get; set; }
        public bool visibleFueraModulo { get; set; }
    }
}
