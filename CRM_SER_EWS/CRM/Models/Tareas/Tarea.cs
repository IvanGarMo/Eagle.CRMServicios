using System.ComponentModel.DataAnnotations;

namespace CRM_SER_EWS.CRM.Models.Tareas
{
    public abstract class TareaBase
    {
        public int idTipoTarea { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe especificar un nombre de la tarea")]
        [StringLength(100, ErrorMessage = "El campo nombre no puede exceder de 100 caracteres")]
        public string nombre { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe especificar una descripción de la tarea")]
        [StringLength(300, ErrorMessage = "El campo descripción no puede exceder de 300 caracteres")]
        public string descripcion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe especificar un color")]
        [StringLength(20, ErrorMessage = "El campo color no puede exceder de 20 caracteres")]
        public string color { get; set; }
        
        public bool visibleFueraModulo { get; set; }

        public TareaBase()
        {
            idTipoTarea = 0;
            nombre = String.Empty;
            descripcion = String.Empty;
            color = String.Empty;
            visibleFueraModulo = false;
        }
    }

    public class Tarea : TareaBase
    {
        public Tarea() : base() { }
    }

    public class TareaEntity : TareaBase
    {
        public string usuario { get; set; }
        public bool activo { get; set; }

        public TareaEntity() : base()
        {
            usuario = String.Empty;
            activo = true;
        }
    }

    public class TareaEstado
    {
        public int idEstado { get; set; }
        public string descripcion { get; set; }
        public string color { get; set; }

        public TareaEstado()
        {
            idEstado = 1;
            descripcion = "Sin iniciar";
            color = "grey";
        }
    }
}
