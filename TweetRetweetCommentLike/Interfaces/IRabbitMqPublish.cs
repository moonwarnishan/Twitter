namespace TweetRetweetCommentLike.Interfaces
{
    public interface IRabbitMqPublish
    {
        public Task Send(Tweet tweet);
    }
}
