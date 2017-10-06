using AutoMapper;
using System.Collections.Generic;

namespace Nsc.Wu.BLL.Mapper
{
    public class AutoMapperHelper<TSource, TDestination>
    {
        public static TDestination MapModel(TSource model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDestination>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<TSource, TDestination>(model);
        }
        public static TDestination MapModel(TSource model, MapperConfiguration config)
        {
            

            IMapper mapper = config.CreateMapper();
            return mapper.Map<TSource, TDestination>(model);
        }
        public static List<TDestination> MapList(List<TSource> list, MapperConfiguration config)
        {
           

            IMapper mapper = config.CreateMapper();
            return mapper.Map<List<TSource>, List<TDestination>>(list);
        }
    }
}
