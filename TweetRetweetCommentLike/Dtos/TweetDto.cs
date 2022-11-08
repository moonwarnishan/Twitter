namespace TweetRetweetCommentLike.Dtos
{
    public class TweetDto
    {
        [BsonId]
        public string? tweetId { get; set; }
        [Required]
        public string? userName { get; set; }
        [Required, MinLength(1)]
        public string? tweetText { get; set; }

        [Required]
        public string tweetTime { get; set; } = DateTime.Now.ToString();

        public List<CommentDto> comments { get; set; } = new List<CommentDto>();
        public List<RetweetDto> retweets { get; set; } = new List<RetweetDto>();
        public List<string> likes { get; set; } = new List<string>();
    }
}
