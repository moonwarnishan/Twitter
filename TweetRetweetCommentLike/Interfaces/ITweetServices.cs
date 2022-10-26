namespace TweetRetweetCommentLike.Interfaces
{
    public interface ITweetServices
    {
        public Task createTweet(Tweet tweet);
        public Task DeleteTweet(string userName,string tweetId);
    }
}
