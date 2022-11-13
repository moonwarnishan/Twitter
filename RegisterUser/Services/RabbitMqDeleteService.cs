namespace RegisterUser.Services
{
    public class RabbitMqDeleteService : IRabbitMqDeleteService, IDisposable
    {
        protected IModel _channel;
        protected readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IMongoCollection<TimelineTweets> _timelineCollection;
        private readonly IRedisServices _redisServices;

        public RabbitMqDeleteService(IServiceProvider serviceProvider,
            IOptions<DatabaseSetting.DatabaseSetting> DBsetting,
            IRedisServices redisServices,
            IRabbitMQService rabbitMQService
            )
        
        {
            _connection = rabbitMQService.CreateChannel();
            _serviceProvider = serviceProvider;
            var client = new MongoClient(DBsetting.Value.connectionString);
            var db = client.GetDatabase(DBsetting.Value.databaseName);
            _timelineCollection = db.GetCollection<TimelineTweets>(DBsetting.Value.userTimelineCollection);
            _redisServices = redisServices;
        }

        public async Task Connect(string userName)
        {

            try
            {
                _channel = _connection.CreateModel();
                var followers = new List<string>();
                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.Received += async (m, e) =>
                {
                    byte[] body = e.Body.ToArray();
                    var msg = (string) Encoding.UTF8.GetString(body)!;
                    var collections = await _timelineCollection.Find(x => x.userName == userName).FirstOrDefaultAsync();
                    var tweet = collections.tweets.Find(x => x.tweetId == msg);
                    collections.tweets.Remove(tweet);
                    await _timelineCollection.ReplaceOneAsync(x => x.userName == userName, collections);
                    await _redisServices.SetCacheValueAsync(userName);
                    await Task.CompletedTask;
                    _channel.BasicAck(e.DeliveryTag, false);
                };

                _channel.BasicConsume(queue: "Dopaminedelete:" + userName,
                    autoAck: true,
                    consumer: consumer);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                
            }
        }
        public void Dispose()
        {
            if (_channel.IsOpen)
                _channel.Close();
            if (_connection.IsOpen)
                _connection.Close();
        }
    }
}
