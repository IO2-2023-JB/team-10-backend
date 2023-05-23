using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities.Models
{
    /// <summary>
    /// Obiekt historii oglądania filmów przez użytkownika
    /// Id historii == Id usera
    /// </summary>
    public class UserHistory : MongoDocumentBase
    {
        public IEnumerable<HistoryItem> WatchedVideos { get; set; }

        public UserHistory(string userId)
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

        private DateTime _date;

        [BsonRepresentation(BsonType.ObjectId)]
        public string VideoId { get; set; }

        public DateTime Date
        {
            get => _date;
            set => _date = value.ToLocalTime();
        }
    }
}
