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

            // ignoruję pola które nie mogą być zmieniane (w PUT video-metadata)
            CreateMap<VideoUpdateDTO, VideoMetadata>()
                .ForMember(video => video.AuthorId,
                           opt => opt.Ignore())
                .ForMember(video => video.AuthorNickname,
                           opt => opt.Ignore())
                .ForMember(video => video.ProcessingProgress,
                           opt => opt.Ignore())
                .ForMember(video => video.ViewCount,
                           opt => opt.Ignore())
                .ForMember(video => video.Duration,
                           opt => opt.Ignore())
                .ForMember(video => video.UploadDate,
                           opt => opt.Ignore());
        }
    }
}
