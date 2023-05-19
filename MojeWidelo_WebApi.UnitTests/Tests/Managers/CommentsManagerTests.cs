using MojeWidelo_WebApi.UnitTests.Mocks;
using Repository.Managers;

namespace MojeWidelo_WebApi.UnitTests.Tests.Managers
{
	public class CommentsManagerTests : BaseManagerTests
	{
		[Fact]
		public async Task CreateCommentArrayTest()
		{
			var id = "64623f1db83bfeff70a313ad";
			var usersRepo = MockIUsersRepository.GetMock().Object;
			var commentsRepo = MockICommentsRepository.GetMock().Object;
			var manager = new CommentManager(GetMapper());

			var comments = (await commentsRepo.GetVideoComments(id)).ToList();
			var users = (await usersRepo.GetUsersByIds(comments.Select(x => x.AuthorId))).ToHashSet();

			var result = manager.CreateCommentArray(comments, users);

			Assert.NotNull(result);
			Assert.Equal(2, result.Length);
		}
	}
}
