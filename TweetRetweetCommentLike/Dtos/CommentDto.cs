namespace TweetRetweetCommentLike.Dtos
{
    public class CommentDto
    {
        [BsonRepresentation(BsonType.ObjectId), BsonId]
        public string commentId { get; set; } = ObjectId.GenerateNewId().ToString();

        [Required]
        public string userName { get; set; }
        
        [Required]
        public string commentText { get; set; }
        [Required]
        public string commentTime { get; set; } = DateTime.Now.ToString();
    }
}
