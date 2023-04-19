namespace Contracts
{
	public interface IVideoRepository : IRepositoryBase<VideoMetadata>
	{
		Task ChangeVideoProcessingProgress(string id, ProcessingProgress uploading);
		void ProccessVideoFile(string id, string path);
		Task<string> UploadThumbnail(string file);
		Task SetThumbnail(HttpContext httpContext, VideoMetadata video, VideoBaseDto videoDto);
		Task<byte[]> GetThumbnailBytes(string id);
		string GetThumbnailContentType(string id);
		Task<IEnumerable<VideoMetadata>> GetVideosByUserId(string id, bool isAuthor);
		Task<IEnumerable<VideoMetadata>> GetSubscribedVideos(IEnumerable<string> creatorIds);
	}
}
