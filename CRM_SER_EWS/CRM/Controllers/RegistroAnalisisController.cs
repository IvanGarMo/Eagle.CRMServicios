using CRM_EWS.CRM.Helpers;
using CRM_EWS.CRM.Models;
using EWS_Contextos.Ventas;
using EWS_SessionManager;
using EWS_SessionManager.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM_EWS.CRM.Controllers
{
    [Autorizado]
    [Route("Analisis")]
    public class RegistroAnalisisController : Controller
    {
        private readonly RegistroAceiteContext context;
        private readonly ICatalogoVentas ventasContext;
        private readonly IConfiguration configuration;

        public RegistroAnalisisController(RegistroAceiteContext context, ICatalogoVentas ventas, IConfiguration configuration)
        {
            this.context = context;
            this.ventasContext = ventas;
            this.configuration = configuration;
        }

        [HttpGet]
        [ActionName("Listado")]
        public IActionResult ListadoAnalisis(QueryRegistroAnalisisAceite query)
        {
            var analisis = context.analisis
                .Where(r => String.IsNullOrWhiteSpace(query.idCliente) || r.idCliente.Equals(query.idCliente))
                .Where(r => (query.tipoAnalisis == 0) || (r.tipoAnalisis == query.tipoAnalisis))
                .Where(r => r.fecha >= query.fechaInicio && r.fecha <= query.fechaFin);
            analisis = query.IsSortAscending ? analisis.OrderBy(r => r.fecha) : analisis.OrderByDescending(r => r.fecha);
            var totalResultados = analisis.Count();

            var clientes = this.ventasContext.CargaCliente();
            var vendedores = this.ventasContext.CargaVendedor().Where(v => v.idEmpresa.Equals("EAGLE"));
            var tipoAnalisis = new ConfiguracionContext(this.configuration).analisis.IgnoreQueryFilters();

            analisis = analisis.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
            
            var listaAnalisis = analisis.ToList();
            var listaTipoAnalisis = tipoAnalisis.ToList();

            var viewModelqueryable = from a in listaAnalisis
                                     join c in clientes on a.idCliente equals c.idCliente
                                     join v in vendedores on a.idVendedor equals  v.idVendedor
                                     join t in listaTipoAnalisis on a.tipoAnalisis equals t.idAnalisis
                                     select new RegistroAnalisisAceiteViewModel
                                     {
                                        idRegistro = a.idRegistro,
                                        idCliente = a.idCliente,
                                        fecha = a.fecha,
                                        tipoAnalisis = a.tipoAnalisis,
                                        idVendedor = a.idVendedor, 
                                        sucursal = a.sucursal,
                                        activo = a.activo, 
                                        usuario = a.usuario, 
                                        nombreCliente = c.nombre,
                                        nombreVendedor = v.nombre, 
                                        descripcionTipoAnalisis = t.descripcion
                                     };

            var resultado = new ResultadoPaginado<RegistroAnalisisAceiteViewModel>(viewModelqueryable.ToList(), query.Page, totalResultados, query.PageSize);
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
