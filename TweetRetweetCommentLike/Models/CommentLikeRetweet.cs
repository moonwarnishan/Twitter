namespace TweetRetweetCommentLike.Models
{
    public class CommentLikeRetweet
    {
        [BsonId]
        public string tweetId { get; set; }

        public List<CommentDto> comments { get; set; } = new List<CommentDto>();
        public List<string> likes { get; set; } = new List<string>();
        public List<RetweetDto> retweets { get; set; } = new List<RetweetDto>();
    }
}
