using Entities.Models;

namespace Contracts
{
	public interface IPlaylistRepository : IRepositoryBase<Playlist>
	{
		public Task<IEnumerable<Playlist>> GetPlaylistByUserId(string id, string callerID);
	}
}
