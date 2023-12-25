using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Mapper
{
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DTOMapper>(); //oprofile bizdeki Profiledan miras alan mapperlar
            });
            return config.CreateMapper();
        });

        //bu metodu çağırmak için bu aşağıdaki metodu oluşturduk
        public static IMapper Mapper => lazy.Value;  //mapper get metodu lazy.value döner.
    }
}