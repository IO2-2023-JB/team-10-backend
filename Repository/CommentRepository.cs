using Contracts;
using Entities.Utils;
using Entities.Models;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using System.Security.Cryptography.X509Certificates;

namespace Repository
{
	public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
	{
		public CommentRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.CommentCollectionName) { }

		public async Task<List<Comment>> GetVideoComments(string id)
		{
			return await _collection.Find((x) => x.VideoId == id && x.OriginCommentId == null).ToListAsync();
		}

		public async Task<List<Comment>> GetCommentResponses(string id)
		{
			return await _collection.Find((x) => x.OriginCommentId == id).ToListAsync();
		}

		public void DeleteCommentResponses(string id)
		{
			_collection.Find((x) => x.OriginCommentId == id).ToList().ForEach(async (x) => await Delete(x.Id));
		}
	}
}
