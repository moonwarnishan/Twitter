namespace TweetRetweetCommentLike.Interfaces
{
    public interface INotificationServices
    {
        public Task CreateNotification(NotificationDto notification);

        public Task DeleteNotification(string tweetId, string userName);

        public Task DeleteNotification(NotificationDto notification);
        public Task<List<NotificationDto>> GetNotification(string userName, int page);
    }
}
