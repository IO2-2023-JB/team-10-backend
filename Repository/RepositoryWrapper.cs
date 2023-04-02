using Contracts;

namespace Repository
{
	public class RepositoryWrapper : IRepositoryWrapper
	{
		public IUsersRepository UsersRepository { get; }
		public IVideoRepository VideoRepository { get; }

		public RepositoryWrapper(IUsersRepository usersRepository, IVideoRepository videoRepository)
		{
			UsersRepository = usersRepository;
			VideoRepository = videoRepository;
		}
	}
}
