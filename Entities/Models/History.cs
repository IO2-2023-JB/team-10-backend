using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Models
{
	/// <summary>
	/// Obiekt historii oglądania filmów przez użytkownika
	/// Id historii == Id usera
	/// </summary>
	public class History : MongoDocumentBase
	{
		public IEnumerable<HistoryItem> WatchedVideos { get; set; }

		public History(string userId)
		{
			Id = userId;
			WatchedVideos = new List<HistoryItem>();
		}
	}

	public class HistoryItem
	{
		public HistoryItem(string videoId)
		{
			VideoId = videoId;
			Date = DateTime.Now;
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string VideoId { get; set; }

		public DateTime Date { get; set; }
	}
}
