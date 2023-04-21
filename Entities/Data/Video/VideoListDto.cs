namespace Entities.Data.Video
{
	public class VideoListDto
	{
		public VideoListDto(IEnumerable<VideoMetadataDto> videos)
		{
			Videos = videos.ToArray();
		}

		public VideoMetadataDto[] Videos { get; set; }
	}
}
