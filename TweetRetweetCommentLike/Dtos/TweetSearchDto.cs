namespace TweetRetweetCommentLike.Dtos
{
    public class TweetSearchDto
    {
        public string key { get; set; }

        public string userName { get; set; }
        public string tweetText { get; set; }
        [BsonId]
        public string tweetId { get; set; }
    }
}
