namespace TweetRetweetCommentLike.Dtos
{
    public class RetweetDto
    {

        [BsonRepresentation(BsonType.ObjectId), BsonId]
        public string? retweetId { get; set; }
        [Required]
        public string? userName { get; set; }
        [Required]
        public DateTime retweetTime { get; set; } = DateTime.Now;
    }
}
