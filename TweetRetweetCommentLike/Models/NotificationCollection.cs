namespace TweetRetweetCommentLike.Models
{
    public class NotificationCollection
    {
        [BsonId]
        public string userName { get; set; }
        public List<NotificationDto> notifications { get; set; } = new List<NotificationDto>();
    }
}
