﻿using CRM_EWS.CRM.Helpers;
using CRM_EWS.CRM.Models;
using CRM_EWS.CRM.Models.Tareas;
using CRM_EWS.Servicios;
using CRM_SER_EWS.CRM.Helpers;
using EWS_SessionManager;
using EWS_SessionManager.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using Utilerias = CRM_EWS.CRM.Helpers.Utilerias;

namespace CRM_EWS.CRM.Controllers
{
    [Autorizado]
    [Route("Tarea")]
    public class RegistroTareasController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccesor;
        public RegistroTareasController(IHttpContextAccessor accessor, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.httpContextAccesor = accessor;
        }

        [ActionName("RegistroTareaCrear")]
        [HttpPost]
        public IActionResult Index([FromBody] RegistroTarea tarea)
        {
            if (Utilerias.ListadoErrores(this.ModelState) is not null)
            {
                var rvm = new ResponseViewModel(1, 0, null);
                rvm.o = Utilerias.ListadoErrores(this.ModelState);
                return BadRequest(rvm);
            }

            var mapper = MapperConfig.InitializaAutomapper();
            var tareaEntity = mapper.Map<RegistroTareaEntity>(tarea);

            using (RegistroTareaContext rt = new RegistroTareaContext(configuration))
            {
                using (IDbContextTransaction tranc = rt.Database.BeginTransaction())
                {
                    try
                    {
                        var username = Utilerias.GetUserName(this.Request);
                        var idUsuario = CRM_EWS.Servicios.Utilerias.GetUserId(this.Request);
                        var tienePermisoTareasOtros = new ProveedorPerfil(this.httpContextAccesor, this.configuration).TienePermiso(username, "SERVTAREAS_OTROS");

                        //Veo que la tarea exista y que se pueda modificar
                        if (tareaEntity.idTarea > 0)
                        {
                            var antTarea = rt.tareas
                                .Where(t => t.idTarea == tarea.idTarea)
                                .FirstOrDefault();

                            if (antTarea == null || antTarea.estado == EstadoTarea.Finalizada)
                            {
                                tranc.Rollback();
                                return StatusCode(400);
                            }

                            var empleadosAsignados = rt.relacionTareaEmpleados
                                .Where(ea => ea.idTarea == tarea.idTarea)
                                .ToList();
                            
                            if(!empleadosAsignados.Select(e => e.idEmpleado).ToList().Contains(idUsuario) && !tienePermisoTareasOtros)
                            {
                                return Unauthorized();
                            }

                            //Actualizo la informacion
                            antTarea.estado = tarea.estado;
                            antTarea.fechaInicio = tarea.fechaInicio;
                            antTarea.fechaFin = tarea.fechaFin;
                            antTarea.descripcion = tarea.descripcion;
                            antTarea.comentarios = tarea.comentarios;
                            antTarea.notas = tarea.notas;
                            antTarea.checkList = tarea.checkList;
                            antTarea.noRequiereCliente = tarea.noRequiereCliente;
                            antTarea.idCliente = tarea.idCliente;

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

                            tareaEntity.idTarea = idMaximaTarea;
                            foreach(CheckList chkTarea in tareaEntity.checkList)
                            {
                                chkTarea.terminado = false;
                            }
                            rt.tareas.Add(tareaEntity);

                            //Añado a los empleados asignados
                            for (int i = 0; i < tareaEntity.empleados.Count; i++)
                            {
                                rt.relacionTareaEmpleados.Add(new RelacionTareaEmpleado(tareaEntity.idTarea, tareaEntity.empleados[i]));
                            }
                            rt.SaveChanges();
                        }

                        tranc.Commit();
                    }
                    catch(Exception ex)
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

        [ActionName("RegistroTConsultar")]
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

            var username = Utilerias.GetUserName(this.Request);
            var idUsuario = CRM_EWS.Servicios.Utilerias.GetUserId(this.Request);
            var tienePermisoTareasOtros = new ProveedorPerfil(this.httpContextAccesor, this.configuration).TienePermiso(username, "SERVTAREAS_OTROS");

            using (RegistroTareaContext rt = new RegistroTareaContext(configuration))
            {
                IQueryable<RegistroTareaVista> tareas = rt.tareasVista
                    .Where(t => (query.rangoFechas) || (query.rangoFechas == false && query.fechaEspecifica.Date == t.fechaFin.Date))
                    .Where(t => (query.rangoFechas == false) || (query.rangoFechas && t.fechaFin >= query.fechaInicioBusqueda && t.fechaFin <= query.fechaFinBusqueda))
                    .Where(t => query.tiposTarea.Count == 0 || query.tiposTarea.Contains(t.idTipoTarea))
                    .Where(t => query.estadoTarea.Count == 0 || query.estadoTarea.Contains(t.idEstado))
                    .Where(t => query.empleados.Count == 0 || query.empleados.Contains(t.idEmpleado))
                    .Where(t => (tienePermisoTareasOtros) || query.empleados.Contains(idUsuario));

                tareas = tareas.ApplyOrdering(query, columnsMap);
                var totalResultados = tareas.Count();
                tareas = tareas.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
                var resultado = new ResultadoPaginado<RegistroTareaVista>(tareas.ToList(), query.Page, totalResultados, query.PageSize);
                return Ok(resultado);
            }
        }
    }
}
