using Contracts;
using Entities.Data.Comment;
using Entities.Data.Video;
using Entities.DatabaseUtils;
using Entities.Enums;
using Entities.Models;
using System.Security.Cryptography.X509Certificates;

namespace Repository
{
	public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
	{
		public CommentRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.CommentCollectionName) { }

		public async Task<List<Comment>> GetVideoComments(string id)
		{
			return (await GetAll()).Where((x) => x.VideoId == id && x.OriginCommentId == null).ToList();
		}

		public async Task<List<Comment>> GetCommentResponses(string id)
		{
			return (await GetAll()).Where((x) => x.OriginCommentId == id).ToList();
		}

		public async Task DeleteCommentResponses(string id)
		{
			(await GetAll())
				.ToList()
				.ForEach(
					async (x) =>
					{
						if (x.OriginCommentId == id)
							await Delete(x.Id);
					}
				);
		}
	}
}
