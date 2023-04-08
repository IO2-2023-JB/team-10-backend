using Entities.Data.Video;
using Entities.Models;

namespace MojeWidelo_WebApi.Mapper
{
	public class VideoProfile : AutoMapper.Profile
	{
		public VideoProfile()
		{
			CreateMap<VideoUploadDto, VideoMetadata>()
				.ForMember(video => video.Tags, opt => opt.MapFrom(video => video.Tags.Distinct()));
			CreateMap<VideoMetadata, VideoMetadataDto>();

			// ignoruję pola które nie mogą być zmieniane (w PUT video-metadata)
			CreateMap<VideoUpdateDto, VideoMetadata>()
				.ForMember(video => video.AuthorId, opt => opt.Ignore())
				.ForMember(video => video.AuthorNickname, opt => opt.Ignore())
				.ForMember(video => video.ProcessingProgress, opt => opt.Ignore())
				.ForMember(video => video.ViewCount, opt => opt.Ignore())
				.ForMember(video => video.Duration, opt => opt.Ignore())
				.ForMember(video => video.UploadDate, opt => opt.Ignore())
				.ForMember(video => video.Tags, opt => opt.MapFrom(video => video.Tags.Distinct()));

			CreateMap<ReactionDto, Reaction>();
        }
    }
}
