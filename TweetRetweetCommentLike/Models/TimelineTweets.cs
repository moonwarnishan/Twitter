using System.Collections;

namespace TweetRetweetCommentLike.Models
{
    public class TimelineTweets 
    {
        [BsonId]
        public string userName { get; set; }
        public List<Tweet> tweets { get; set; }
    }
}
