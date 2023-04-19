using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;

namespace Contracts
{
	public interface ICommentRepository : IRepositoryBase<Comment>
	{
		Task<List<Comment>> GetVideoComments(string id);
	}
}
