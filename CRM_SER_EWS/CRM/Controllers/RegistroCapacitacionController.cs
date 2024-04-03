using CRM_SER_EWS.CRM.Helpers;
using CRM_SER_EWS.CRM.Models.Capacitaciones;
using EWS_SessionManager.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace CRM_SER_EWS.CRM.Controllers
{
    [Route("Capacitacion")]
    public class RegistroCapacitacionController : Controller
    {
        private readonly IConfiguration configuration;

        public RegistroCapacitacionController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("{idCapacitacion}")]
        public IActionResult Consultar(int idCapacitacion)
        {
            using(var context = new RegistroCapacitacionContext(configuration))
            {
                var curso = context.capacitaciones.FirstOrDefault(c => c.idCapacitacion == idCapacitacion);
                if(curso is null)
                {
                    return NotFound();
                }
                //var instructoresQueryable = from emp in context.empleados
                //                     join cap in context.capacitadores on emp.idEmpleado equals cap.idEmpleado into ce
                //                     from e in ce.DefaultIfEmpty()
                //                     where e.idCapacitacion == idCapacitacion
                //                     select new CapacitacionEmpleado(idCapacitacion, e.idEmpleado, emp.nombre, (bool) e.activo);

                var result = from e in context.empleados
                             join c in context.capacitadores
                             on e.idEmpleado equals c.idEmpleado into grupo
                             from a in grupo.DefaultIfEmpty()
                             select new
                             {
                                 EmpleadoId = e.idEmpleado, 
                                 Nombre = e.nombre, 
                                 Activo = a.activo == null ? false : a.activo
                             };


                return Ok(result.ToList());
            }
        }

        [HttpPost]
        public IActionResult Index([FromBody] Capacitacion capacitacion)
        {
            using (var context = new RegistroCapacitacionContext(configuration))
            {
                using (IDbContextTransaction tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        int idCapacitacion = 0;
                        //Es inserción de nuevo registro
                        if (capacitacion.idCapacitacion == 0)
                        {
                            try
                            {
                                idCapacitacion = context.capacitaciones.Select(c => c.idCapacitacion).Max();
                                idCapacitacion += 1;
                            }
                            catch (InvalidOperationException ex)
                            {
                                idCapacitacion = 1;
                            }
                            capacitacion.idCapacitacion = idCapacitacion;
                            context.capacitaciones.Add(capacitacion);
                            context.SaveChanges();
                        }
                        else
                        {
                            var antCapacitacion = context.capacitaciones.FirstOrDefault(c => c.idCapacitacion == capacitacion.idCapacitacion);
                            if (antCapacitacion is null)
                            {
                                tran.Rollback();
                                return NotFound();
                            }
                            antCapacitacion.asistentes = capacitacion.asistentes;
                            antCapacitacion.sucursal = capacitacion.sucursal;
                            antCapacitacion.incluyeCertificado = capacitacion.incluyeCertificado;
                            antCapacitacion.descripcion = capacitacion.descripcion;
                            idCapacitacion = antCapacitacion.idCapacitacion;
                        }

                        //Guardo a los instructores
                        var instructoresAnt = context.capacitadores.Where(ce => ce.idCapacitacion == idCapacitacion);
                        context.capacitadores.RemoveRange(instructoresAnt);
                        context.SaveChanges();
                        for (int i = 0; i < capacitacion.instructores.Count; i++)
                        {
                            context.capacitadores.Add(new CapacitacionEmpleado(idCapacitacion, capacitacion.instructores[i].idEmpleado, true));
                        }
                        context.SaveChanges();

                        //Si la capacitación fue indicada como de tipo Chevron, se cargarán todos los módulos
                        //Si no, solamente los que indique el usuario
                        if (capacitacion.incluyeCertificado)
                        {
                            using (var contextConfiguration = new ConfiguracionContext(configuration))
                            {
                                var curso = contextConfiguration.cursos.FirstOrDefault(c => c.idCurso.Equals(capacitacion.idCurso));
                                if (curso == null)
                                {
                                    tran.Rollback();
                                    var rvm = new ResponseViewModel(1, 0, "El curso indicado no existe");
                                    return BadRequest(rvm);
                                }

                                if (!curso.cursoChevron)
                                {
                                    tran.Rollback();
                                    var rvm = new ResponseViewModel(1, 0, "El curso indicado no está catalogado como un curso Chevron");
                                    return BadRequest(rvm);
                                }

                                var submodulosAnt = context.submodulos.Where(s => s.idCapacitacion == idCapacitacion);
                                context.submodulos.RemoveRange(submodulosAnt);
                                context.SaveChanges();

                                var submodulosId = (from modulo in contextConfiguration.modulosCurso
                                                    join submodulo in contextConfiguration.submodulosCurso
                                                    on modulo.idModulo equals submodulo.idModulo
                                                    where modulo.idCurso == capacitacion.idCurso
                                                    select submodulo.idSubmodulo);
                                foreach (int i in submodulosId)
                                {
                                    context.submodulos.Add(new CapacitacionSubmodulo(idCapacitacion, i, true));
                                }
                            }
                        }
                        else
                        {
                            var submodulosAnt = context.submodulos.Where(s => s.idCapacitacion == idCapacitacion);
                            context.submodulos.RemoveRange(submodulosAnt);
                            context.SaveChanges();
                            for (int i = 0; i < capacitacion.Submodulos.Count; i++)
                            {
                                context.submodulos.Add(new CapacitacionSubmodulo(idCapacitacion, capacitacion.Submodulos[i].idSubmodulo, true));
                            }
                        }
                        context.SaveChanges();

                        tran.Commit();
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        var rvm = new ResponseViewModel(1, 0, "Ha habido un error");
                        rvm.AnadeMensaje(ex.Message);
                        return StatusCode(500, rvm);
                    }
                }
            }
        }
    }
}
