using Contracts;

namespace Repository
{
	public class RepositoryWrapper : IRepositoryWrapper
	{
		public IUsersRepository UsersRepository { get; }
		public IVideoRepository VideoRepository { get; }
		public IReactionRepository ReactionRepository { get; }
		public ICommentRepository CommentRepository { get; }
		public ISubscriptionsRepository SubscriptionsRepository { get; }
		public IPlaylistRepository PlaylistRepository { get; }
		public IHistoryRepository HistoryRepository { get; }
		public ITicketRepository TicketRepository { get; }

		public RepositoryWrapper(
			IUsersRepository usersRepository,
			IVideoRepository videoRepository,
			IReactionRepository reactionRepository,
			ICommentRepository commentRepository,
			ISubscriptionsRepository subscriptionsRepository,
			IPlaylistRepository playlistRepository,
			IHistoryRepository historyRepository,
			ITicketRepository ticketRepository
		)
		{
			UsersRepository = usersRepository;
			VideoRepository = videoRepository;
			ReactionRepository = reactionRepository;
			CommentRepository = commentRepository;
			SubscriptionsRepository = subscriptionsRepository;
			PlaylistRepository = playlistRepository;
			HistoryRepository = historyRepository;
			TicketRepository = ticketRepository;
		}
	}
}
