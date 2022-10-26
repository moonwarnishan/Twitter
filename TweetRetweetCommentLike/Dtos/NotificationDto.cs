namespace TweetRetweetCommentLike.Dtos
{
    public class NotificationDto
    {
        public string receiverUserName { get; set; }
        public string senderUserName { get; set; }
        public string message { get; set; }
        public string tweetId { get; set; }
        public string time { get; set; } = DateTime.Now.ToString();
    }
}
