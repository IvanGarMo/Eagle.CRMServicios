using CRM_SER_EWS.CRM.Helpers;
using CRM_SER_EWS.CRM.Models;
using CRM_SER_EWS.Servicios;
using EWS_SessionManager;
using EWS_SessionManager.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM_SER_EWS.CRM.Controllers
{
    [Autorizado]
    [Route("Catalogo/Curso")]
    public class CatalogoCursosController : ControllerBase
    {
        private readonly ConfiguracionContext configuracion;

        public CatalogoCursosController(ConfiguracionContext configuracion)
        {
            this.configuracion = configuracion;
        }

        [ActionName("Curso")]
        [HttpGet]
        public IActionResult Curso(CursoQuery query)
        {

        var columnsMap = new Dictionary<string, Expression<Func<Curso, object>>>
            {
                ["idCurso"] = c => c.idCurso,
                ["nombre"] = c => c.nombre,
                ["cursoChevron"] = c => c.cursoChevron,
                ["fechaCreacion"] = c => c.fechaCreacion,
                ["activo"] = c => c.activo
            };
            IQueryable<Curso> lista = configuracion.cursos
                .Where(c => c.nombre.Contains(query.nombre));
            lista = lista.ApplyOrdering(query, columnsMap);
            var totalResultados = lista.Count();
            lista = lista.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
            var resultado = new ResultadoPaginado<Curso>(lista.ToList(), query.Page, totalResultados, query.PageSize);
            return Ok(resultado);
        }

        [ActionName("CursoCrea")]
        [HttpPost]
        public IActionResult CursoCrear([FromBody] Curso curso)
        {
            if (curso is null)
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
                curso.usuario = Utilerias.GetUserName(this.Request);
                curso.fechaCreacion = DateTime.Now;
                curso.activo = true;
                configuracion.cursos.Add(curso);
                configuracion.SaveChanges();
                return Ok();
            } catch(Exception ex)
            {
                var rvm = new ResponseViewModel(1, 0, "Ha habido un error");
                rvm.AnadeMensaje(ex.Message);
                return StatusCode(500, rvm);
            }
        }

        [ActionName("Modulo")]
        [HttpGet]
        [Route("{idCurso}/Modulo")]
        public IActionResult Modulos(int idCurso)
        {
            var listaModulos = configuracion.modulosCurso.Where(c => c.idCurso == idCurso && c.activo);
            if(listaModulos is null || listaModulos.Count() == 0)
            {
                return Ok();
            }
            return Ok(listaModulos);
        }

        [ActionName("ModuloCrea")]
        [HttpPost]
        [Route("{idCurso}/Modulo")]
        public IActionResult CrearModulo(int idCurso, [FromBody] CursoModulo modulo)
        {
            if (modulo is null)
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
                modulo.idCurso = idCurso;
                modulo.activo = true;
                configuracion.modulosCurso.Add(modulo);
                configuracion.SaveChanges();
                return Ok();
            } 
            catch(DbUpdateException ex) when (((SqlException) (ex.InnerException)).Number == 537)
            {
                var rvm = new ResponseViewModel(1, 0, "El curso al que se intenta capturar no existe");
                return StatusCode(500, rvm);
            } catch(Exception ex)
            {
                var rvm = new ResponseViewModel(1, 0, "Ha habido un error");
                rvm.AnadeMensaje(ex.Message);
                return StatusCode(500, rvm);
            }
        }

        [ActionName("Submodulo")]
        [HttpGet]
        [Route("{idModulo}/Submodulo")]
        public IActionResult Submodulos(int idModulo)
        {
            var listaSubmodulos = configuracion.submodulosCurso.Where(c => c.idModulo == idModulo && c.activo);
            if (listaSubmodulos is null || listaSubmodulos.Count() == 0)
            {
                return Ok();
            }
            return Ok(listaSubmodulos);
        }

        [ActionName("SubmoduloCrea")]
        [HttpPost]
        [Route("{idModulo}/Submodulo")]
        public IActionResult SubmoduloCrea(int idModulo, [FromBody] CursoSubmodulo submodulo)
        {
            if (submodulo is null)
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
                submodulo.activo = true;
                submodulo.idModulo = idModulo;
                configuracion.submodulosCurso.Add(submodulo);
                configuracion.SaveChanges();
                return Ok();
            } 
            catch(Exception ex)
            {
                var rvm = new ResponseViewModel(1, 0, "Ha habido un error");
                rvm.AnadeMensaje(ex.Message);
                return StatusCode(500, rvm);
            }
        }
    }
}
