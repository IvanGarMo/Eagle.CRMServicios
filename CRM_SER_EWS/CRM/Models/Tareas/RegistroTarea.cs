using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CRM_EWS.CRM.Models.Tareas
{
    public abstract class RegistroTareaBase
    {
        public int idTarea { get; set; }

        [AsignacionTarea(ErrorMessage = "Es necesario que especifique un cliente, o indique dicha tarea no va asignada a un cliente en especifico")]
        [MaxLength(10, ErrorMessage = "El campo id cliente no puede exceder de 10 caracteres")]
        public string idCliente { get; set; }
        public bool noRequiereCliente { get; set; }

        [Required(ErrorMessage = "Es necesario que indique el tipo de tarea")]
        public int idTipoTarea { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario que especifique una sucursal")]
        [MaxLength(30, ErrorMessage = "El campo sucursal no puede exceder de 30 caracteres")]
        public string sucursal { get; set; }
        public EstadoTarea estado { get; set; }
        public DateTime fechaInicio { get; set; }

        [FechaFin(ErrorMessage = "El campo fecha inicio no puede ser posterior a la fecha fin")]
        public DateTime fechaFin { get; set; }

        [MaxLength(400, ErrorMessage = "El campo descripción no puede exceder de 400 caracteres")]
        public string descripcion { get; set; }
        
        [MaxLength(400, ErrorMessage = "El campo comentarios no puede exceder de 400 caracteres")]
        public string comentarios { get; set; }
        
        [MaxLength(400, ErrorMessage = "El campo notas no puede exceder de 400 caracteres")]
        public string notas { get; set; }
        
        public List<CheckList> checkList { get; set; }
       
        public List<int> empleados { get; set; }

        public RegistroTareaBase()
        {
            idTarea = -1;
            idCliente = String.Empty;
            idTipoTarea = -1;
            sucursal = String.Empty;
            estado = EstadoTarea.EnCurso;
            fechaInicio = DateTime.Now;
            fechaFin = DateTime.Now;
            descripcion = String.Empty;
            comentarios = String.Empty;
            notas = String.Empty;
            checkList = new List<CheckList>();
            empleados = new List<int>();
            noRequiereCliente = false;
        }
        
    }

    public class RegistroTarea : RegistroTareaBase
    {
        public RegistroTarea() : base() {  }  
    }

    public class RegistroTareaEntity : RegistroTareaBase
    {
        public RegistroTareaEntity() : base() { }

        public bool activo { get; set; }
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
    }

    public class CheckList
    {
        public string descripcion { get; set; }
        public bool terminado { get; set; }

        public CheckList()
        {
            descripcion = String.Empty;
            terminado = false;
        }
    }

    public class TareaEmpleados
    {
        public int idTarea { get; set; }
        public List<Int32> idEmpleado { get; set; }

        public TareaEmpleados()
        {
            idEmpleado = new List<Int32>();
        }
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

    internal class FechaFin : ValidationAttribute
    {
 
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            RegistroTarea rt = (RegistroTarea)validationContext.ObjectInstance;
            if (rt.fechaInicio > rt.fechaFin)
            {
                return new ValidationResult(this.ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }

    internal class AsignacionTarea : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            RegistroTarea rt = (RegistroTarea)validationContext.ObjectInstance;
            if (!rt.noRequiereCliente && String.IsNullOrWhiteSpace(rt.idCliente))
            {
                return new ValidationResult(this.ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
