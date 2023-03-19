using Entities.Data;
using Entities.Models;

namespace MojeWidelo_WebApi.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UpdateUserDTO, User>();
            CreateMap<RegisterDTO, User>();
        }
    }
}
