using Entities.Enums;

namespace MojeWidelo_WebApi.UnitTests.Mocks
{
	public class MockUser
	{
		public static string Id { get; } = "6429a1ee0d48bf254e17eaf7";
		public static UserType UserType { get; } = UserType.Creator;
		public static string Nickname { get; } = "Mocked nickname";
	}
}
