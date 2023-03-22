using Contracts;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        public IUsersRepository UsersRepository { get; }

        public RepositoryWrapper(IUsersRepository usersRepository)
        {
            UsersRepository = usersRepository;
        }
    }
}
