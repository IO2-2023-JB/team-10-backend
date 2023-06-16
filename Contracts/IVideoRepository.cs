using Entities.Data.Playlist;
using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;

namespace Contracts
{
	public interface IVideoRepository : IRepositoryBase<VideoMetadata>
	{
		Task ChangeVideoProcessingProgress(string id, ProcessingProgress uploading);
		Task SetThumbnail(VideoMetadata video, VideoBaseDto videoDto);
		Task ProccessVideoFile(string id, string path);
		Task<byte[]> GetThumbnailBytes(string id);
		Task<IEnumerable<VideoMetadata>> GetVideosByUserId(string id, bool isAuthor);
		Task<IEnumerable<VideoMetadata>> GetSubscribedVideos(IEnumerable<string> creatorIds);
		Task<IEnumerable<VideoMetadata>> GetVideos(IEnumerable<string> videosIDs, string userID);
		Task<IEnumerable<VideoMetadata>> GetAllVisibleVideos(string userId);
		Task<VideoMetadata> UpdateViewCount(string id, int value);
		Task<string> GetDuration(string id);
		Task UpdateVideoDuration(string id, string duration);
		Task ProcessAndAddDuration(string id, string path);
		Task<IEnumerable<VideoMetadata>> GetMoreVideosToRecommend(
			IEnumerable<RecommendationDto> videoIDs,
			string userId,
			IEnumerable<Subscription> userSubscriptions
		);
		Task<string?> DeleteVideo(string id);
	}
}
