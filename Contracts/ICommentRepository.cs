using Entities.Models;

namespace Contracts
{
	public interface ICommentRepository : IRepositoryBase<Comment>
	{
		Task<List<Comment>> GetVideoComments(string id);
		Task<List<Comment>> GetCommentResponses(string id);
		Task DeleteCommentResponses(string id);
		Task DeleteVideoComments(string videoId);
		Task<IEnumerable<Comment>> GetCommentsByUserId(string id);
	}
}
