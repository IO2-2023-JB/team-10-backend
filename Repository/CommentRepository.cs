﻿using Contracts;
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
			return await Collection.Find((x) => x.VideoId == id && x.OriginCommentId == null).ToListAsync();
		}

		public async Task<List<Comment>> GetCommentResponses(string id)
		{
			return await Collection.Find((x) => x.OriginCommentId == id).ToListAsync();
		}

		public async Task DeleteCommentResponses(string id)
		{
			await Collection.DeleteManyAsync(x => x.OriginCommentId == id);
		}

		public async Task DeleteVideoComments(string videoId)
		{
			await Collection.DeleteManyAsync(x => x.VideoId == videoId);
		}

		public async Task<IEnumerable<Comment>> GetCommentsByUserId(string id)
		{
			return await Collection.Find(x => x.AuthorId == id).ToListAsync();
		}
	}
}
