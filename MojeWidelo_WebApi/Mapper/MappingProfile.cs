namespace MojeWidelo_WebApi.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UpdateUserDTO, User>();
            CreateMap<RegisterRequestDTO, User>()
                .ForMember(user => user.Password,
                           opt => opt.MapFrom(register => HashHelper.HashPassword(register.Password)));
            CreateMap<VideoUploadDTO, VideoMetadata>();
            CreateMap<VideoMetadata, VideoMetadataDTO>();
        }
    }
}
