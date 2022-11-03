namespace TweetRetweetCommentLike.Interfaces
{
    public interface IGetTweetServices
    {
        public Task<List<TweetDto>> GetTimelineTweets(string userName,int page);
        public Task RemoveTweet(string followedName, string userName);
        public Task<TweetDto> getTweetbyId(string userName, string TweetId);
        public Task<List<TweetDto>> GeTweetsbyuserName(string userName, int page);
        public Task<List<TweetDto>> GetTimelineTweetsRedis(string userName);

    }
}
