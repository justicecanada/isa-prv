using AutoMapper;
using Interview.Entities;
using Interview.UI.Models;

namespace Interview.UI.Services.Automapper
{
    
    public class MapperConfig : Profile
    {

        public MapperConfig()
        {

            CreateMap<Contest, VmContest>();
            CreateMap<VmContest, Contest>();


        }

    }

}
