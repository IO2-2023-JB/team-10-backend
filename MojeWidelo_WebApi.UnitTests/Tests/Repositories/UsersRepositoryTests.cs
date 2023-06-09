﻿using Entities.Models;
using MojeWidelo_WebApi.UnitTests.Mocks;

namespace MojeWidelo_WebApi.UnitTests.Tests.Repositories
{
	public class UsersRepositoryTests
	{
		[Theory]
		[InlineData("unit@test.com")]
		[InlineData("user2@test.com")]
		public async void FindUserByEmailTest(string email)
		{
			var usersRepoMock = MockIUsersRepository.GetMock();
			var result = await usersRepoMock.Object.FindUserByEmail(email);

			Assert.NotNull(result);
			Assert.Equal(email, result.Email);
			Assert.IsAssignableFrom<User>(result);
		}

		[Theory]
		[InlineData("jdsiajfiweo@test.com")]
		[InlineData("halohalo@halo.com")]
		public async void FindUserByNonExistingEmailTest(string email)
		{
			var usersRepoMock = MockIUsersRepository.GetMock();
			var result = await usersRepoMock.Object.FindUserByEmail(email);

			Assert.Null(result);
		}
	}
}
