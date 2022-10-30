namespace TweetRetweetCommentLike.DatabaseSetting
{
    public class DatabaseSetting
    {
        public string followBlockCollectionName { get; set; }
        public string followeeBlockeeCollectionName { get; set; }
        public string hashSearchCollection { get; set; }
        public string userTimelineCollection { get; set; }
        public string likeCommentRetweetCollectionName { get; set; }
        public string notificationCollectionName { get; set; }
        public string connectionString { get; set; }
        public string databaseName { get; set; }
    }
}
