using Contracts;
using Entities.Data.Comment;
using Entities.Data.Video;
using Entities.DatabaseUtils;
using Entities.Enums;
using Entities.Models;

namespace Repository
{
	public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
	{
		public CommentRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.CommentCollectionName) { }

		public async Task<List<Comment>> GetVideoComments(string id)
		{
			return (await GetAll()).Where((x) => x.VideoId == id).ToList();
		}
	}
}
