using CRM_SER_EWS.CRM.Helpers;
using CRM_SER_EWS.CRM.Models;
using CRM_SER_EWS.CRM.Models.Tareas;
using EWS_SessionManager;
using EWS_SessionManager.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace CRM_SER_EWS.CRM.Controllers
{
    [Route("Tarea")]
    public class RegistroTareasController : Controller
    {
        private readonly IConfiguration configuration;
        public RegistroTareasController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public IActionResult Index([FromBody] RegistroTarea tarea)
        {
            if (tarea is null)
            {
                return BadRequest();
            }

            if (Utilerias.ListadoErrores(this.ModelState) is not null)
            {
                var rvm = new ResponseViewModel(1, 0, null);
                rvm.o = Utilerias.ListadoErrores(this.ModelState);
                return BadRequest(rvm);
            }


            if(tarea.fechaFin < tarea.fechaInicio)
            {
                var rvm = new ResponseViewModel(1, 0, "La fecha de fin no puede ser posterior a la fecha de inicio");
                return BadRequest(rvm);
            }

            using(RegistroTareaContext rt = new RegistroTareaContext(configuration))
            {
                using (IDbContextTransaction tranc = rt.Database.BeginTransaction())
                {
                    try
                    {

                        //Veo que la tarea exista y que se pueda modificar
                        if (tarea.idTarea > 0)
                        {
                            var antTarea = rt.tareas.FirstOrDefault(t => t.idTarea == tarea.idTarea);
                            if (antTarea == null || antTarea.estado == EstadoTarea.Finalizada)
                            {
                                tranc.Rollback();
                                return StatusCode(400);
                            }

                            //Actualizo la informacion
                            antTarea.estado = tarea.estado;
                            antTarea.fechaInicio = tarea.fechaInicio;
                            antTarea.fechaFin = tarea.fechaFin;
                            antTarea.descripcion = tarea.descripcion;
                            antTarea.comentarios = tarea.comentarios;
                            antTarea.notas = tarea.notas;
                            antTarea.checkList = tarea.checkList;

                            //Elimino a los anteriores empleados asignados, para poder volverlo a guardar
                            var emps = rt.relacionTareaEmpleados.Where(r => r.idTarea == antTarea.idTarea);
                            rt.relacionTareaEmpleados.RemoveRange(emps);
                            rt.SaveChanges();

                            for (int i = 0; i < tarea.empleados.Count; i++)
                            {
                                rt.relacionTareaEmpleados.Add(new RelacionTareaEmpleado(tarea.idTarea, tarea.empleados[i]));
                            }
                            rt.SaveChanges();
                        }
                        else
                        {
                            int idMaximaTarea = 0;
                            try
                            {
                                idMaximaTarea = rt.tareas.Select(t => t.idTarea).Max();
                                idMaximaTarea += 1;
                            }
                            catch (InvalidOperationException ex)
                            {
                                idMaximaTarea = 1;
                            }

                            tarea.idTarea = idMaximaTarea;
                            rt.tareas.Add(tarea);

                            //Añado a los empleados asignados
                            for (int i = 0; i < tarea.empleados.Count; i++)
                            {
                                rt.relacionTareaEmpleados.Add(new RelacionTareaEmpleado(tarea.idTarea, tarea.empleados[i]));
                            }
                            rt.SaveChanges();
                        }

                        tranc.Commit();
                    } catch(Exception ex)
                    {
                        tranc.Rollback();
                        var rvm = new ResponseViewModel(1, 0, "Ha habido un error");
                        rvm.AnadeMensaje(ex.Message);
                        return StatusCode(500, rvm);
                    }
                }
                return Ok();
            }
        }

        [HttpGet]
        public IActionResult Consulta(RegistroTareaVistaQuery query)
        {
            var columnsMap = new Dictionary<string, Expression<Func<RegistroTareaVista, object>>>
            {
                ["idTarea"] = r => r.idTarea,
                ["idTipoTarea"] = r => r.idTipoTarea,
                ["fechaInicio"] = r => r.fechaInicio,
                ["fechaFin"] = r => r.fechaFin,
                ["idEmpleado"] = r => r.idEmpleado,
                ["idEstado"] = r => r.idEstado
            };

            using (RegistroTareaContext rt = new RegistroTareaContext(configuration))
            {
                IQueryable<RegistroTareaVista> tareas = rt.tareasVista
                    .Where(t => (query.rangoFechas) || (query.rangoFechas == false && query.fechaEspecifica.Date == t.fechaFin.Date))
                    .Where(t => (query.rangoFechas == false) || (query.rangoFechas && t.fechaFin >= query.fechaInicioBusqueda && t.fechaFin <= query.fechaFinBusqueda))
                    .Where(t => query.tiposTarea.Count == 0 || query.tiposTarea.Contains(t.idTipoTarea))
                    .Where(t => query.estadoTarea.Count == 0 || query.estadoTarea.Contains(t.idEstado))
                    .Where(t => query.empleados.Count == 0 || query.empleados.Contains(t.idEmpleado));

                tareas = tareas.ApplyOrdering(query, columnsMap);
                var totalResultados = tareas.Count();
                tareas = tareas.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
                var resultado = new ResultadoPaginado<RegistroTareaVista>(tareas.ToList(), query.Page, totalResultados, query.PageSize);
                return Ok(resultado);
            }
        }
    }
}
