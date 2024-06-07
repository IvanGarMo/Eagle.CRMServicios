using CRM_EWS.CRM.Helpers;
using CRM_SER_EWS.CRM.Helpers;
using CRM_SER_EWS.CRM.Models.Tareas;
using EWS_SessionManager;
using EWS_SessionManager.Response;
using Microsoft.AspNetCore.Mvc;
using Utilerias = CRM_EWS.CRM.Helpers.Utilerias;


namespace CRM_EWS.CRM.Controllers
{
    [Autorizado]
    [Route("Catalogo/Tarea")]
    public class CatalogoTareaController : Controller
    {
        private readonly ConfiguracionContext _configuracion;

        public CatalogoTareaController(ConfiguracionContext configuracion)
        {
            _configuracion = configuracion;
        }

        [ActionName("Consultar")]
        [HttpGet]
        public IActionResult Index()
        {
            var tareas = _configuracion.tareas.Where<TareaEntity>(t => t.activo);
            return Ok(tareas);
        }

        [ActionName("Consultar")]
        [HttpGet("{id}")]
        public IActionResult CargaInformacion(int id)
        {
            var tarea = _configuracion.tareas.Find(id);
            if (tarea is null || !tarea.activo)
            {
                return NotFound();
            }
            return Ok(tarea);
        }

        [ActionName("Crear")]
        [HttpPost]
        public IActionResult Crear([FromBody] Tarea? t)
        {
            if (Utilerias.ListadoErrores(this.ModelState) is not null)
            {
                var rvm = new ResponseViewModel(1, 0, null);
                rvm.o = Utilerias.ListadoErrores(this.ModelState);
                return BadRequest(rvm);
            }

            try
            {
                var mapper = MapperConfig.InitializaAutomapper();
                var tareaEntity = mapper.Map<TareaEntity>(t);
                //De lo contrario, tengo que ver si es un update o una creación
                if (tareaEntity.idTipoTarea == 0)
                {
                    tareaEntity.usuario = Utilerias.GetUserName(this.Request);
                    tareaEntity.activo = true;
                    _configuracion.tareas.Add(tareaEntity);
                    _configuracion.SaveChanges();
                }
                else
                {
                    var antTarea = _configuracion.tareas.Find(tareaEntity.idTipoTarea);
                    if (antTarea is null || !antTarea.activo)
                    {
                        return NotFound();
                    }
                    antTarea.nombre = tareaEntity.nombre;
                    antTarea.descripcion = tareaEntity.descripcion;
                    antTarea.color = tareaEntity.color;
                    antTarea.activo = tareaEntity.activo;
                    antTarea.visibleFueraModulo = tareaEntity.visibleFueraModulo;
                    _configuracion.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                var rvm = new ResponseViewModel(1, 0, "Ha habido un error");
                rvm.AnadeMensaje(ex.Message);
                return StatusCode(500, rvm);
            }
            
        }

        [ActionName("Eliminar")]
        [HttpDelete("{id}")]

        public IActionResult Eliminar(int id)
        {
            var t = _configuracion.tareas.Find(id);
            if(t is null || !t.activo)
            {
                return NotFound();
            }

            try
            {
                t.activo = false;
                _configuracion.SaveChanges();
                return Ok();
            } catch(Exception ex)
            {
                var rvm = new ResponseViewModel(1, 0, "Ha habido un error");
                rvm.AnadeMensaje(ex.Message);
                return StatusCode(500, rvm);
            }
        }

        [ActionName("Estado")]
        [HttpGet]
        [Route("Estado")]
        public IActionResult TareasEstados()
        {
            var listaEstados = new[] { 
                new { idEstado = 1, descripcion = "Sin iniciar", color = "grey" },
                new { idEstado = 2, descripcion = "En curso", color = "orange" },
                new { idEstado = 3, descripcion = "Completado", color = "green" },
                new { idEstado = 4, descripcion = "Con retraso", color = "red" }
            } ;

            return Ok(listaEstados);
        }
    }
}
