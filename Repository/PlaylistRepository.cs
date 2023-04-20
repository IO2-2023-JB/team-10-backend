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
		readonly PlaylistManager playlistManager;

		public PlaylistRepository(IDatabaseSettings databaseSettings, PlaylistManager playlistManager)
			: base(databaseSettings, databaseSettings.PlaylistCollectionName)
		{
			this.playlistManager = playlistManager;
		}

		public async Task<IEnumerable<Playlist>> GetPlaylistByUserId(string id, string callerID)
		{
			return await _collection
				.Find(x => x.AuthorId == id && (x.Visibility == Visibility.Public || x.AuthorId == callerID))
				.ToListAsync();
		}
	}
}
