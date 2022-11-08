namespace TweetRetweetCommentLike.Interfaces
{
    public interface IRedisServices
    {
        public Task<List<TweetDto>> timelineTweetsFromRedis(string userName);
    }
}
