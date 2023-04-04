using Entities.Data.User;
using Entities.Data.Video;
using Entities.Models;
using MojeWidelo_WebApi.Helpers;

namespace MojeWidelo_WebApi.Mapper
{
	public class MappingProfile : AutoMapper.Profile
	{
		public MappingProfile()
		{
			CreateMap<User, UserDto>()
				.ForMember(
					user => user.SubscriptionsCount,
					opt =>
						opt.MapFrom(
							user => user.UserType == Entities.Enums.UserType.Creator ? user.SubscriptionsCount : null
						)
				);
			CreateMap<UpdateUserDto, User>()
				.ForMember(user => user.Id, opt => opt.Ignore())
				.ForMember(user => user.Email, opt => opt.Ignore())
				.ForMember(user => user.AccountBalance, opt => opt.Ignore())
				.ForMember(user => user.Password, opt => opt.Ignore())
				.ForMember(user => user.SubscriptionsCount, opt => opt.Ignore());

			CreateMap<RegisterRequestDto, User>()
				.ForMember(
					user => user.Password,
					opt => opt.MapFrom(register => HashHelper.HashPassword(register.Password))
				);

			CreateMap<VideoUploadDto, VideoMetadata>();
			CreateMap<VideoMetadata, VideoMetadataDto>();

			// ignoruję pola które nie mogą być zmieniane (w PUT video-metadata)
			CreateMap<VideoUpdateDto, VideoMetadata>()
				.ForMember(video => video.AuthorId, opt => opt.Ignore())
				.ForMember(video => video.AuthorNickname, opt => opt.Ignore())
				.ForMember(video => video.ProcessingProgress, opt => opt.Ignore())
				.ForMember(video => video.ViewCount, opt => opt.Ignore())
				.ForMember(video => video.Duration, opt => opt.Ignore())
				.ForMember(video => video.UploadDate, opt => opt.Ignore());
		}
	}
}
