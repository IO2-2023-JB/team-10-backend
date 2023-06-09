﻿namespace Entities.Utils
{
	public class DatabaseSettings : IDatabaseSettings
	{
		public string DatabaseName { get; set; }
		public string ConnectionString { get; set; }
		public string UsersCollectionName { get; set; }
		public string VideoCollectionName { get; set; }
		public string ReactionCollectionName { get; set; }
		public string CommentCollectionName { get; set; }
		public string SubscriptionCollectionName { get; set; }
		public string PlaylistCollectionName { get; set; }
		public string HistoryCollectionName { get; set; }
		public string TicketCollectionName { get; set; }
	}
}
