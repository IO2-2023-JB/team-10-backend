using Contracts;

namespace Repository
{
	public class RepositoryWrapper : IRepositoryWrapper
	{
		public IUsersRepository UsersRepository { get; }
		public IVideoRepository VideoRepository { get; }
		public IReactionRepository ReactionRepository { get; }
        public ICommentRepository CommentRepository { get; }

        public RepositoryWrapper(
			IUsersRepository usersRepository,
			IVideoRepository videoRepository,
			IReactionRepository reactionRepository,
			ICommentRepository commentRepository
		)
		{
			UsersRepository = usersRepository;
			VideoRepository = videoRepository;
			ReactionRepository = reactionRepository;
			CommentRepository = commentRepository;
		}
	}
}
