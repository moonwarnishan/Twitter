namespace TweetRetweetCommentLike.Interfaces
{
    public interface IGetTweetServices
    {
        public Task<List<TweetDto>> GetTimelineTweets(string userName);
        public Task RemoveTweet(string followedName, string userName);
        public Task<TweetDto> getTweetbyId(string userName, string TweetId);
        public Task<List<TweetDto>> GeTweetsbyuserName(string userName);
        public Task<List<TweetDto>> GetTimelineTweetsRedis(string userName);

    }
}
