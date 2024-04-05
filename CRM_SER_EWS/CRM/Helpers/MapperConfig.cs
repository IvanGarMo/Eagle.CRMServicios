using AutoMapper;
using CRM_EWS.CRM.Models;

namespace CRM_SER_EWS.CRM.Helpers
{
    public class MapperConfig
    {
        public static Mapper InitializaAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegistroAnalisisAceite, RegistroAnalisisAceiteEntity>();
            });

            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
