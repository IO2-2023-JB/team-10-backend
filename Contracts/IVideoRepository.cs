using Entities.Enums;
using Entities.Models;

namespace Contracts
{
	public interface IVideoRepository : IRepositoryBase<VideoMetadata>
	{
		Task ChangeVideoProcessingProgress(string id, ProcessingProgress uploading);
		void ProccessVideoFile(string id, string path);
	}
}
