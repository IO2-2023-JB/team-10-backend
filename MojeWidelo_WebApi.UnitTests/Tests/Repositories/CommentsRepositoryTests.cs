using MojeWidelo_WebApi.UnitTests.Mocks;

namespace MojeWidelo_WebApi.UnitTests.Tests.Repositories
{
	public class CommentsRepositoryTests
	{
		[Theory]
		[InlineData("64390ed1d3768498801aa14f")]
		[InlineData("64390ed1d3768498801aa04f")]
		public async Task GetVideoComments_WhenVideoHasNoComments(string id)
		{
			var repository = MockICommentsRepository.GetMock().Object;
			var result = await repository.GetVideoComments(id);

			Assert.Empty(result);
		}

		[Theory]
		[InlineData("64623f1db83bfeff70a313ad")]
		public async Task GetVideoComments_WhenVideoHasComments(string id)
		{
			var repository = MockICommentsRepository.GetMock().Object;
			var result = await repository.GetVideoComments(id);

			Assert.Equal(2, result.Count);
		}

		[Theory]
		[InlineData("64390ed1d3768498801aa14f")]
		[InlineData("64390ed1d3768498801aa04f")]
		public async Task GetCommentResponses_WhenCommentHasNoResponses(string id)
		{
			var repository = MockICommentsRepository.GetMock().Object;
			var result = await repository.GetCommentResponses(id);

			Assert.Empty(result);
		}

		[Theory]
		[InlineData("645181c02c3e1b16d9dd4420")]
		public async Task GetCommentResponses_WhenCommentHasResponses(string id)
		{
			var repository = MockICommentsRepository.GetMock().Object;
			var result = await repository.GetCommentResponses(id);

			Assert.Single(result);
		}
	}
}
