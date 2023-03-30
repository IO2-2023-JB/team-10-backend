using Entities.Data;
using Entities.Models;
using MojeWidelo_WebApi.Helpers;

namespace MojeWidelo_WebApi.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UpdateUserDTO, User>();
            CreateMap<RegisterDTO, User>()
                .ForMember(user => user.Password,
                           opt => opt.MapFrom(register => HashHelper.HashPassword(register.Password)));
        }
    }
}
