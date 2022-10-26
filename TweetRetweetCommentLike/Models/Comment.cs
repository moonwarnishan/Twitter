
namespace TweetRetweetCommentLike.Models
{
    public class Comment
    {
        [BsonRepresentation(BsonType.ObjectId), BsonId]
        public string? commentId { get; set; }
        [Required]
        public string? userName { get; set; }
        [Required, MinLength(1)]
        public string? commentText { get; set; }
        [Required]
        public DateTime? commentTime { get; set; } = DateTime.Now;

    }
}
