using Contracts;
using Entities.Enums;
using Entities.Models;
using Entities.Utils;
using MongoDB.Driver;

namespace Repository
{
	public class PlaylistRepository : RepositoryBase<Playlist>, IPlaylistRepository
	{
		public PlaylistRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.PlaylistCollectionName) { }

		public async Task<IEnumerable<Playlist>> GetPlaylistByUserId(string id, User caller)
		{
			return await Collection
				.Find(
					x =>
						x.AuthorId == id
						&& (
							x.Visibility == PlaylistVisibility.Public
							|| x.AuthorId == caller.Id
							|| caller.UserType == UserType.Administrator
						)
				)
				.ToListAsync();
		}

		public async Task<IEnumerable<Playlist>> GetAllVisiblePlaylists(string userId)
		{
			return await Collection
				.Find(x => x.Visibility == PlaylistVisibility.Public || x.AuthorId == userId)
				.ToListAsync();
		}
	}
}
