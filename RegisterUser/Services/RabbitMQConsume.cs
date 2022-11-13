namespace RegisterUser.Services
{
    public class RabbitMqConsume : IRabbitMQConsume, IDisposable
    {
        protected IModel _channel;
        private readonly IConnection _connection;
        protected readonly IServiceProvider _serviceProvider;
        private readonly IRedisServices _redisServices;
        private readonly IMongoCollection<TimelineTweets> _timelineCollection;
        public RabbitMqConsume(IServiceProvider serviceProvider,
            IOptions<DatabaseSetting.DatabaseSetting> DBsetting,
            IRedisServices redisServices,
            IRabbitMQService mqService
            )
        {
            _connection = mqService.CreateChannel();
            _serviceProvider = serviceProvider;
            var client = new MongoClient(DBsetting.Value.connectionString);
            var db = client.GetDatabase(DBsetting.Value.databaseName);
            _timelineCollection = db.GetCollection<TimelineTweets>(DBsetting.Value.userTimelineCollection);
            _redisServices = redisServices;
        }

        public async Task Connect(string userName)
        {

            _channel = _connection.CreateModel();
            var followers = new List<string>();
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (m, e) =>
            {
                byte[] body = e.Body.ToArray();
                var msg = (Tweet)JsonConvert.DeserializeObject<Tweet>(Encoding.UTF8.GetString(body))!;
                var collections = await _timelineCollection.Find(x => x.userName == userName).FirstOrDefaultAsync();
                collections.tweets.Insert(0, msg);
                await _timelineCollection.ReplaceOneAsync(x => x.userName == userName, collections);
                await _redisServices.SetCacheValueAsync(userName);
                await Task.CompletedTask;
                _channel.BasicAck(e.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "Dopamine:" + userName,
                autoAck: true,
                consumer: consumer);
            await Task.CompletedTask;
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
