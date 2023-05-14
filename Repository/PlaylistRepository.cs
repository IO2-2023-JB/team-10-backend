using Contracts;
using Entities.Enums;
using Entities.Models;
using Entities.Utils;
using MongoDB.Driver;
using Repository.Managers;

namespace Repository
{
	public class PlaylistRepository : RepositoryBase<Playlist>, IPlaylistRepository
	{
		public PlaylistRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.PlaylistCollectionName) { }

		public async Task<IEnumerable<Playlist>> GetPlaylistByUserId(string id, string callerID)
		{
			return await Collection
				.Find(x => x.AuthorId == id && (x.Visibility == PlaylistVisibility.Public || x.AuthorId == callerID))
				.ToListAsync();
		}
	}
}
