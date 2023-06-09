﻿namespace Contracts
{
	public interface IRepositoryWrapper
	{
		IUsersRepository UsersRepository { get; }
		IVideoRepository VideoRepository { get; }
		IReactionRepository ReactionRepository { get; }
		ICommentRepository CommentRepository { get; }
		ISubscriptionsRepository SubscriptionsRepository { get; }
		IPlaylistRepository PlaylistRepository { get; }
		IHistoryRepository HistoryRepository { get; }
		ITicketRepository TicketRepository { get; }
	}
}
