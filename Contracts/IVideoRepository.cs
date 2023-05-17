using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Http;

namespace Contracts
{
	public interface IVideoRepository : IRepositoryBase<VideoMetadata>
	{
		Task ChangeVideoProcessingProgress(string id, ProcessingProgress uploading);
		void ProccessVideoFile(string id, string path);
		Task SetThumbnail(VideoMetadata video, VideoBaseDto videoDto);
		Task<byte[]> GetThumbnailBytes(string id);
		Task<IEnumerable<VideoMetadata>> GetVideosByUserId(string id, bool isAuthor);
		Task<IEnumerable<VideoMetadata>> GetSubscribedVideos(IEnumerable<string> creatorIds);
		Task<IEnumerable<VideoMetadata>> GetVideos(IEnumerable<string> videosIDs, string userID);
		Task<IEnumerable<VideoMetadata>> GetAllVisibleVideos(string userId);
		Task<VideoMetadata> UpdateViewCount(string id, int value);
	}
}
