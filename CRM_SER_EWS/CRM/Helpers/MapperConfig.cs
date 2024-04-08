using AutoMapper;
using CRM_EWS.CRM.Models;
using CRM_EWS.CRM.Models.Tareas;

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
            });

            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
