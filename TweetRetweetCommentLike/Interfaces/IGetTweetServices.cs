namespace TweetRetweetCommentLike.Interfaces
{
    public interface IGetTweetServices
    {
        public Task<List<Tweet>> GetTimelineTweets(string userName);
    }
}
