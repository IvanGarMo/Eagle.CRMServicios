using AutoMapper;
using CRM_EWS.CRM.Models;
using CRM_EWS.CRM.Models.Equipos;
using CRM_EWS.CRM.Models.Tareas;
using CRM_SER_EWS.CRM.Models.Tareas;

namespace CRM_SER_EWS.CRM.Helpers
{
    public class MapperConfig
    {
        public static Mapper InitializaAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegistroAnalisisAceite, RegistroAnalisisAceiteEntity>();
                cfg.CreateMap<RegistroTarea, RegistroTareaEntity>();
                cfg.CreateMap<Tarea, TareaEntity>();
                cfg.CreateMap<Equipo, EquipoEntity>();
            });

            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
