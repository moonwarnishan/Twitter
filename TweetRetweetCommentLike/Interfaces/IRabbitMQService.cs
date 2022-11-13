namespace TweetRetweetCommentLike.Interfaces
{
    public interface IRabbitMQService
    {
        public IConnection CreateChannel();
    }
}
