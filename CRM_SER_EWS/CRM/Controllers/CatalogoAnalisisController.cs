using CRM_EWS.Analisis;
using CRM_EWS.CRM.Helpers;
using EWS_SessionManager;
using EWS_SessionManager.Response;
using Microsoft.AspNetCore.Mvc;

namespace CRM_EWS.CRM.Controllers
{
    [Autorizado]
    [Route("Catalogo/Analisis")]
    public class CatalogoAnalisisController : Controller
    {
        private readonly ConfiguracionContext _configuracion;

        public CatalogoAnalisisController(ConfiguracionContext configuracion)
        {
            _configuracion = configuracion;
        }

        [ActionName("Consultar")]
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var analisis = _configuracion.analisis.ToList<AnalisisAceite>();
                return Ok(analisis);
            } catch(Exception ex)
            {
                var rvm = new ResponseViewModel(1, 0, "Ha habido un error");
                rvm.AnadeMensaje(ex.Message);
                return StatusCode(500, rvm);
            }
        }

        [ActionName("ConsultarEspecifico")]
        [HttpGet("{id}")]
        public IActionResult CargaDetalle(int id)
        {
            try
            {
                var analisis = _configuracion.analisis.Find(id);
                if (analisis is null)
                {
                    return NotFound();
                }
                return Ok(analisis);
            } catch(Exception ex)
            {
                var rvm = new ResponseViewModel(1, 0, "Ha habido un error");
                rvm.AnadeMensaje(ex.Message);
                return StatusCode(500, rvm);
            }
        }

        [ActionName("Escribir")]
        [HttpPost]
        public IActionResult CreaActualiza([FromBody] AnalisisAceite analisis)
        {
            if(analisis is null)
            {
                return BadRequest();
            }

            if(Utilerias.ListadoErrores(this.ModelState) is not null)
            {
                var rvm = new ResponseViewModel(1, 0, null);
                rvm.o = Utilerias.ListadoErrores(this.ModelState);
                return BadRequest(rvm);
            }

            try
            {
                if (analisis.idAnalisis == 0)
                {
                    analisis.usuario = Utilerias.GetUserName(this.Request);
                    analisis.activo = true;
                    _configuracion.analisis.Add(analisis);
                    _configuracion.SaveChanges();
                }
                else
                {
                    var antAnalisis = _configuracion.analisis.Find(analisis.idAnalisis);
                    if (antAnalisis is null)
                    {
                        return NotFound();
                    }
                    antAnalisis.nombre = analisis.nombre;
                    antAnalisis.descripcion = analisis.descripcion;
                    antAnalisis.moneda = analisis.moneda;
                    antAnalisis.costo = analisis.costo;
                    _configuracion.SaveChanges();
                }
                return Ok();
            } catch(Exception ex)
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
            try
            {
                var analisis = _configuracion.analisis.Find(id);
                if (analisis is null)
                {
                    return NotFound();
                }
                analisis.activo = false;
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
