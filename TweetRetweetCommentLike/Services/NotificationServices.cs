namespace TweetRetweetCommentLike.Services
{
    public class NotificationServices : INotificationServices
    {
        private readonly IMongoCollection<NotificationCollection> _notifications;
        private readonly IRabbitMqNotification _mqNotification;
        public NotificationServices(IOptions<DatabaseSetting.DatabaseSetting> db, IRabbitMqNotification mqNotification)
        {
            var client = new MongoClient(db.Value.connectionString);
            var database = client.GetDatabase(db.Value.databaseName);
            _notifications = database.GetCollection<NotificationCollection>(db.Value.notificationCollectionName);
            _mqNotification = mqNotification;
        }
        //create
        public async Task CreateNotification(NotificationDto notification)
        {
            await _mqNotification.Send(notification);
            var collection = await _notifications.FindAsync(x => x.userName == notification.receiverUserName).Result.FirstOrDefaultAsync();
            if (collection == null)
            {
                var newCollection = new NotificationCollection
                {
                    userName = notification.receiverUserName,
                    notifications = new List<NotificationDto> { notification }
                };
                await _notifications.InsertOneAsync(newCollection);
            }
            else
            {
                collection.notifications.Insert(0,notification);
                await _notifications.ReplaceOneAsync(x => x.userName == notification.receiverUserName, collection);
            }
        }

        //Delete a tweets All notification
        public async Task DeleteNotification(string tweetId, string userName)
        {
            var collection = await _notifications.FindAsync(x => x.userName == userName).Result.FirstOrDefaultAsync();
            if (collection != null)
            {
                collection.notifications.RemoveAll(x => x.tweetId == tweetId);
                await _notifications.ReplaceOneAsync(x => x.userName == userName, collection);
            }
        }

        public async Task DeleteNotification(NotificationDto notification)
        {
            var collection = await _notifications.FindAsync(x => x.userName == notification.receiverUserName).Result.FirstOrDefaultAsync();
            collection.notifications.Remove(notification);
            await _notifications.ReplaceOneAsync(x => x.userName == notification.receiverUserName, collection);
            
        }

        public async Task<List<NotificationDto>> GetNotification(string userName,int page)
        {
            var collection = await _notifications.FindAsync(x => x.userName == userName).Result.FirstOrDefaultAsync();
            
            if (collection != null)
            {
                
                return collection.notifications.Take(10*page).ToList();
            }

            return null;

        }



    }
}
