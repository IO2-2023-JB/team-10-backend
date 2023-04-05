using AutoMapper;
using Contracts;
using Entities.Data.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Controllers;
using MojeWidelo_WebApi.Mapper;
using MojeWidelo_WebApi.UnitTests.Mocks;

namespace MojeWidelo_WebApi.UnitTests
{
	public class VideoRepositoryTests
	{
		[Fact]
		public void GetDirectoryLocationTest()
		{
			var repositoryWrapperMock = MockIRepositoryWrapper.GetMock();

			string? path = repositoryWrapperMock.Object.VideoRepository.GetStorageDirectory();
			string? path2 = Environment.GetEnvironmentVariable("MojeWideloStorage", EnvironmentVariableTarget.Machine);
			Assert.Equal(path, path2);
		}
	}
}
