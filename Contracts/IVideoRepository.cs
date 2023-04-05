using Entities.Enums;
using Entities.Models;

namespace Contracts
{
	public interface IVideoRepository : IRepositoryBase<VideoMetadata>
	{
		Task<bool> ChangeVideoProcessingProgress(string id, ProcessingProgress uploading);
		string? CreateNewPath(string id, string fileName);
		string? GetReadyFilePath(string id);
		void ProccessVideoFile(string id, string path);
		public string? GetStorageDirectory();
	}
}
