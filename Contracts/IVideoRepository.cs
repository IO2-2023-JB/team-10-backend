using Entities.Enums;
using Entities.Models;

namespace Contracts
{
	public interface IVideoRepository : IRepositoryBase<VideoMetadata>
	{
		void ChangeVideoProcessingProgress(string id, ProcessingProgress uploading);
		string? CreateNewPath(string id, string fileName);
	}
}
