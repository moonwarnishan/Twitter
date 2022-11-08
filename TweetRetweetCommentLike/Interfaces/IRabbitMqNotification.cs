namespace TweetRetweetCommentLike.Interfaces
{
    public interface IRabbitMqNotification
    {
        public Task Send(NotificationDto Notification);
    }
}
