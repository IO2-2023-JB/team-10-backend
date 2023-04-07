namespace Contracts
{
	public interface IRepositoryWrapper
	{
		IUsersRepository UsersRepository { get; }
		IVideoRepository VideoRepository { get; }
        IReactionRepository ReactionRepository{ get; }
    }
}
