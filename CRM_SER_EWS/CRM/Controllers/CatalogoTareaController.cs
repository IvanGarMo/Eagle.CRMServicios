using CRM_EWS.CRM.Helpers;
using CRM_EWS.Servicios;
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
            var tareas = _configuracion.tareas.Where<Tarea>(t => t.activo);
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
            if(t is null)
            {
                return BadRequest();
            }

            if (Utilerias.ListadoErrores(this.ModelState) is not null)
            {
                var rvm = new ResponseViewModel(1, 0, null);
                rvm.o = Utilerias.ListadoErrores(this.ModelState);
                return BadRequest(rvm);
            }

            try
            {
                //De lo contrario, tengo que ver si es un update o una creación
                if (t.idTarea == 0)
                {
                    t.usuario = Utilerias.GetUserName(this.Request);
                    t.activo = true;
                    _configuracion.tareas.Add(t);
                    _configuracion.SaveChanges();
                }
                else
                {
                    var antTarea = _configuracion.tareas.Find(t.idTarea);
                    if (antTarea is null || !antTarea.activo)
                    {
                        return NotFound();
                    }
                    antTarea.nombre = t.nombre;
                    antTarea.descripcion = t.descripcion;
                    antTarea.color = t.color;
                    antTarea.activo = t.activo;
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
    }
}
