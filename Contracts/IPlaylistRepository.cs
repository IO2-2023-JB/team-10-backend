using Entities.Models;

namespace Contracts
{
	public interface IPlaylistRepository : IRepositoryBase<Playlist>
	{
		Task<IEnumerable<Playlist>> GetPlaylistByUserId(string id, User caller);
		Task<IEnumerable<Playlist>> GetAllVisiblePlaylists(string userId);
	}
}
