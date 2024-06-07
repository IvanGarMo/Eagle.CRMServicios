using CRM_EWS.CRM.Helpers;
using CRM_EWS.CRM.Models;
using CRM_EWS.CRM.Models.Equipos;
using CRM_EWS.CRM.Models.Tareas;
using CRM_SER_EWS.CRM.Helpers;
using EWS_ContextoGobernador;
using EWS_ContextoGobernador.Contexto;
using EWS_ContextoGobernador.Operaciones;
using EWS_Contextos.Ventas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace CRM_EWS.CRM.Controllers
{
    [Route("Equipos")]
    public class EquipoController : ControllerBase
    {
        private readonly EquipoContext context;
        private readonly ICatalogoVentas ventasContext;
        private readonly IConfiguration configuration;
        private readonly GobernadorContext gobernadorContext;

        public EquipoController(EquipoContext context, ICatalogoVentas ventasContext, IConfiguration configuration)
        {
            this.context = context;
            this.ventasContext = ventasContext;
            this.configuration = configuration;
            this.gobernadorContext = gobernadorContext;
        }

        [HttpGet]
        [Route("Estados")]
        public IActionResult EstadosEquipo()
        {
            var values = new[] {
                new { estado = 0, descripcion = "TODOS" },
                new { estado = 1, descripcion = "ASIGNADO" },
                new { estado = 2, descripcion = "SIN ASIGNAR" }
            };
            return Ok(values);
        }

        [HttpGet]
        public IActionResult Equipos(QueryEquipos query)
        {
            var queryableSucursalesEagle = context.sucursales.Where(s => s.idEmpresa.Equals("EAGLE"));

            var equiposQueryable = from e in context.equipos
                                   join s in queryableSucursalesEagle on e.sucursalActual equals s.idSucursal
                                   join z in context.zonas on s.idZona equals z.idZona
                                   join r in context.regiones on z.idRegion equals r.idRegion
                                   where (String.IsNullOrWhiteSpace(query.Region) || query.Region.Equals(query.Region))
                                   where (query.zona.Count == 0 || query.zona.Contains(z.idZona))
                                   where (query.Sucursal.Count == 0 || query.Sucursal.Contains(e.sucursalActual))
                                   where (query.Estatus == 0 || (query.Estatus == 1 && e.asignado) || (query.Estatus == 2 && !e.asignado))
                                   select new EquipoIndice
                                   {
                                       idActF = e.idActf,
                                       nombreEquipo = e.nombreEquipo,
                                       idZona = s.idZona,
                                       descripcionZona = z.zona,
                                       sucursal = e.sucursalActual,
                                       asignado = e.asignado
                                   };
            equiposQueryable = equiposQueryable.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
            var listaEquipos = equiposQueryable.ToList();
            var resultado = new ResultadoPaginado<EquipoIndice>(listaEquipos, query.Page, listaEquipos.Count, query.PageSize);
            return Ok(resultado);
        }

        //[HttpPost]
        //public IActionResult CrearActualizarEquipo(Equipo equipo)
        //{
        //    if (equipo.idActf == 0)
        //    {
        //        var mapper = MapperConfig.InitializaAutomapper();
        //        var equipoEntity = mapper.Map<EquipoEntity>(equipo);
        //        equipoEntity.usuarioAlta = equipoEntity.usuarioFum = Utilerias.GetUserName(this.Request);
        //        equipoEntity.fechaAlta = equipoEntity.fechaFum = DateTime.Now;
        //        equipo.idActf = (context.equipos.Max<EquipoEntity, Nullable<Int32>>(e => e.idActf) ?? 0) + 1;
        //        context.equipos.Add(equipoEntity);
        //        context.SaveChanges();
        //    }
        //    else
        //    {
                
        //    }

        //}

        [HttpGet]
        [Route("{idEquipo}")]
        public IActionResult Equipo(int idEquipo)
        {
            var equipo = context.equipos
                .Where(e => e.idActf == idEquipo)
                .FirstOrDefault();

            if(equipo is null)
            {
                return NotFound();
            }

            var traslados = context.traslados
                .Where(et => et.idActF == idEquipo)
                .ToList();


            var equipoPrestamos = context.prestamos.Where(e => e.idActF == idEquipo).ToList();
            var queryableClientes = ventasContext.CargaCliente();
            var listaPrestamos = from ep in equipoPrestamos
                                 join qc in queryableClientes on ep.idCliente equals qc.idCliente
                                 select new
                                 {
                                     idPrestamo = ep.idPrestamo,
                                     idActF = ep.idActF,
                                     idCliente = ep.idCliente,
                                     nombreCliente = qc.idCliente, 
                                     fechaInicio = ep.fechaInicio,
                                     idTareaInstalacion = ep.idTareaInstalacion, 
                                     prestamoActivo = ep.prestamoActivo, 
                                     idFechaFin = ep.fechaFin,
                                     idTareaRetiro = ep.idTareaRetiro
                                 };

            return Ok(new
            {
                idActf = equipo.idActf, 
                nombreEquipo = equipo.nombreEquipo, 
                numSerie = equipo.numSerie,
                costoCompra = equipo.costoCompra, 
                fechaCompra = equipo.fechaCompra, 
                modelo = equipo.modelo, 
                marca = equipo.marca, 
                sucursalCompra = equipo.sucursalCompra, 
                sucursalActual = equipo.sucursalActual, 
                asignado = equipo.asignado,
                traslados = traslados,
                prestamos = listaPrestamos
            });
        }

        [HttpPost]
        [Route("Asignacion")]
        public IActionResult CapturarMovimiento(EquipoMovimiento em)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Cargo el prestamo activo
            var equipoInstalado = context.prestamos.Where(e => e.idActF == em.idActf && e.prestamoActivo).FirstOrDefault();
            if(em.esInstalacion && equipoInstalado != null)
            {
                return BadRequest();
            }

            //Si es retiro, verifico que efectivamente esté instalado
            if (em.esRetiro && equipoInstalado == null)
            {
                return BadRequest();
            }

            var equipo = context.equipos.Where(e => e.idActf == em.idActf).FirstOrDefault();
            var usuario = "IVAN";
            //Procedo a registrar
            //Debo registrar la instalación o retiro, y luego la tarea correspondiente
            using (var tran = context.Database.BeginTransaction())
            {
                try
                {
                    using (var tareasContext = new RegistroTareaContext(this.configuration))
                    {
                        tareasContext.Database.SetDbConnection(context.Database.GetDbConnection());
                        tareasContext.Database.UseTransaction(tran.GetDbTransaction());

                        int idMaximaTarea = 0;
                        try
                        {
                            idMaximaTarea = tareasContext.tareas.Select(t => t.idTarea).Max();
                            idMaximaTarea += 1;
                        }
                        catch (InvalidOperationException ex)
                        {
                            idMaximaTarea = 1;
                        }
                        var nuevaTarea = new RegistroTarea
                        {
                            idTarea = idMaximaTarea,
                            idCliente = em.idCliente,
                            idTipoTarea = em.idTipoTarea,
                            sucursal = equipo.sucursalActual,
                            estado = EstadoTarea.NoIniciada,
                            fechaInicio = em.fechaInstalacionRetiro,
                            fechaFin = em.fechaInstalacionRetiro,
                            descripcion = String.Empty,
                            comentarios = String.Empty,
                            notas = String.Empty,
                            checkList = new List<CheckList>()
                        };
                        tareasContext.Add(nuevaTarea);
                        //Añado a los empleados asignados
                        for (int i = 0; i < em.empleados.Count; i++)
                        {
                            tareasContext.relacionTareaEmpleados.Add(new RelacionTareaEmpleado(nuevaTarea.idTarea, em.empleados[i]));
                        }
                        tareasContext.SaveChanges();

                        if (em.esRetiro)
                        {
                            equipo.asignado = false;
                            equipoInstalado.prestamoActivo = false;
                            equipoInstalado.fechaFin = em.fechaInstalacionRetiro;
                            equipoInstalado.idTareaRetiro = idMaximaTarea;
                            context.SaveChanges();
                        }
                        else
                        {
                            int folio = 0;
                            string idMovimiento = GenerarFolio(context.Database.GetDbConnection(), tran, em, usuario, equipo.sucursalActual, out folio);
                            context.prestamos.Add(new EquipoPrestamo()
                            {
                                idPrestamo = idMovimiento,
                                idActF = em.idActf,
                                idCliente = em.idCliente,
                                fechaInicio = em.fechaInstalacionRetiro,
                                idTareaInstalacion = idMaximaTarea,
                                prestamoActivo = true,
                                fechaFin = null,
                                idTareaRetiro = null
                            });

                            equipo.asignado = true;
                            context.SaveChanges();
                        }
                    }
                    tran.Commit();
                    return Ok();
                } 
                catch(Exception ex)
                {
                    tran.Rollback();
                    return StatusCode(500);
                }
            }
        }

        private string GenerarFolio(DbConnection connection, IDbContextTransaction tran, EquipoMovimiento em, string usuario, string sucursal, out int folio)
        {
            using (var operacionesContext = new OperacionesContext(configuration))
            {
                operacionesContext.Database.SetDbConnection(connection);
                operacionesContext.Database.UseTransaction(tran.GetDbTransaction());
                var gobernadorContext = new Gobernador(this.configuration);
                var empresa = gobernadorContext.CargaEmpresas(usuario).Where(e => e.idEmpresa.Equals("EAGLE")).FirstOrDefault();
                var sucursalEmpresa = gobernadorContext.CargaSucursalesUsuario(empresa.idEmpresa, usuario).Where(s => s.idSucursal.Equals(sucursal)).FirstOrDefault();
                var cveEmpresa = empresa.clave;
                var cveSucursal = sucursalEmpresa.clave;
                Operaciones op = null;

                try
                {
                    op = operacionesContext.operaciones.Where(o => o.tipo.Equals("MOVTOEQUIPO") && o.idEmpresa.Equals("EAGLE") && o.idSucursal.Equals(sucursal)).FirstOrDefault();
                    if(op is null)
                    {
                        op = new Operaciones();
                        op.idEmpresa = "EAGLE";
                        op.idSucursal = sucursal;
                        op.tipo = "MOVTOEQUIPO";
                        op.nombre = "MOVTOEQUIPO";
                        op.enUso = "SI";
                        op.folio = 1;
                        folio = 1;
                        op.fechaAnt1 = op.fechaAnt2 = op.fechaAnt3 = op.fum = DateTime.Now;
                        op.usuariofum = "EFFRAMEWORK";
                        operacionesContext.operaciones.Add(op);
                    } else
                    {
                        op.folio += 1;
                        op.fechaAnt1 = op.fechaAnt2 = op.fechaAnt3 = op.fum;
                        op.fum = DateTime.Now;
                        op.usuariofum = "EFFRAMEWORK";
                        folio = (int)op.folio;
                    }   
                }
                catch(Exception ex)
                {
                    throw ex;
                }

                var idSalida = String.Concat(cveEmpresa, cveSucursal, "MEQ", new String('0', (8 - op.folio.ToString().Count())), op.folio.ToString());
                return idSalida;
            }
        }
    }
}