using Contracts;

namespace Repository
{
	public class RepositoryWrapper : IRepositoryWrapper
	{
		public IUsersRepository UsersRepository { get; }
		public IVideoRepository VideoRepository { get; }
        public IReactionRepository ReactionRepository{ get; }

        public RepositoryWrapper(IUsersRepository usersRepository, IVideoRepository videoRepository, IReactionRepository reactionRepository)
		{
			UsersRepository = usersRepository;
			VideoRepository = videoRepository;
			ReactionRepository = reactionRepository;
		}
	}
}
