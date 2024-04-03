using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CRM_SER_EWS.CRM.Models.Tareas
{
    public class RegistroTarea
    {
        public int idTarea { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario que especifique un cliente")]
        public string idCliente { get; set; }
        [Required(ErrorMessage = "Es necesario que indique el tipo de tarea")]
        public int idTipoTarea { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario que especifique una sucursal")]
        public string sucursal { get; set; }
        public EstadoTarea estado { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        [MaxLength(400, ErrorMessage = "El campo descripción no puede exceder de 400 caracteres")]
        public string descripcion { get; set; }
        [MaxLength(400, ErrorMessage = "El campo comentarios no puede exceder de 400 caracteres")]
        public string comentarios { get; set; }
        [MaxLength(400, ErrorMessage = "El campo notas no puede exceder de 400 caracteres")]
        public string notas { get; set; }
        public List<CheckList> checkList { get; set; }
        public string checkListSerialized
        {
            get
            {
                return JsonConvert.SerializeObject(checkList);
            }
            set
            {
                checkList = string.IsNullOrWhiteSpace(value) ? new List<CheckList>() : JsonConvert.DeserializeObject<List<CheckList>>(value);
            }
        }
        public List<int> empleados { get; set; }


        public RegistroTarea()
        {
            checkList = new List<CheckList>();
            empleados = new List<int>();
            descripcion = string.Empty;
            comentarios = string.Empty;
            notas = string.Empty;
            estado = EstadoTarea.NoIniciada;
            fechaInicio = DateTime.Today;
            fechaFin = DateTime.Today.AddDays(1);
            checkListSerialized = string.Empty;
        }
    }

    public class CheckList
    {
        public string descripcion { get; set; }
        public bool terminado { get; set; }
    }

    public class RelacionTareaEmpleado
    {
        public int idTarea { get; set; }
        public int idEmpleado { get; set; }

        public RelacionTareaEmpleado(int idTarea, int idEmpleado)
        {
            this.idTarea = idTarea;
            this.idEmpleado = idEmpleado;
        }
    }

    public enum EstadoTarea
    {
        NoIniciada = 1,
        EnCurso = 2,
        Finalizada = 3
    }
}
