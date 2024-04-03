using CRM_SER_EWS.CRM.Helpers;
using CRM_SER_EWS.CRM.Models;
using EWS_SessionManager;
using EWS_SessionManager.Response;
using Microsoft.AspNetCore.Mvc;

namespace CRM_SER_EWS.CRM.Controllers
{
    [Autorizado]
    [Route("Analisis")]
    public class RegistroAnalisisController : Controller
    {
        private readonly RegistroAceiteContext context;

        public RegistroAnalisisController(RegistroAceiteContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [ActionName("Listado")]
        public IActionResult ListadoAnalisis(QueryRegistroAnalisisAceite query)
        {
            IQueryable<RegistroAnalisisAceite> analisis = context.analisis
                .Where(r => String.IsNullOrWhiteSpace(query.idCliente) || r.idCliente.Equals(query.idCliente))
                .Where(r => (query.tipoAnalisis == 0) || (r.tipoAnalisis == query.tipoAnalisis))
                .Where(r => r.fecha >= query.fechaInicio && r.fecha <= query.fechaFin);
            analisis = query.IsSortAscending ? analisis.OrderBy(r => r.fecha) : analisis.OrderByDescending(r => r.fecha);
            var totalResultados = analisis.Count();

            analisis = analisis.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
            var resultado = new ResultadoPaginado<RegistroAnalisisAceite>(analisis.ToList(), query.Page, totalResultados, query.PageSize);
            return Ok(resultado);
        }

        [HttpPost]
        [ActionName("Captura")]
        public IActionResult RegistraAnalisis([FromBody] RegistroAnalisisAceite registro)
        {
            if (registro is null)
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
                if(registro.idRegistro == 0)
                {
                    registro.usuario = Utilerias.GetUserName(this.Request);
                    registro.activo = true;
                    context.analisis.Add(registro);
                    context.SaveChanges();
                }
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
