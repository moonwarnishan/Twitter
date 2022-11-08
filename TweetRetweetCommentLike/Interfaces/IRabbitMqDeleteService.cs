namespace TweetRetweetCommentLike.Interfaces
{
    public interface IRabbitMqDeleteService
    {
        public Task Send(string userName, string tweetId);
    }
}
