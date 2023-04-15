namespace Entities.Utils
{
	public class DatabaseSettings : IDatabaseSettings
	{
		public string DatabaseName { get; set; }
		public string ConnectionString { get; set; }
		public string UsersCollectionName { get; set; }
		public string VideoCollectionName { get; set; }
		public string ReactionCollectionName { get; set; }
	}
}
