using Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Video
{
    public class ReactionResponseDto
    {
        public int positiveCount { get; set; }

        public int negativeCount { get; set; }
    }
}
