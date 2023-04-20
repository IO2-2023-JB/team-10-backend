using Contracts;
using Entities.Models;
using Entities.Utils;
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
    }
}
