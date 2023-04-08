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
		Task<string> UploadThumbnail(string file);
		Task SetThumbnail(HttpContext httpContext, VideoMetadata video, VideoBaseDto videoDto);
		Task<byte[]> GetThumbnailBytes(string id);
	}
}
