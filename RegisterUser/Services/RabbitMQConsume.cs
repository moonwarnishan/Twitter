namespace RegisterUser.Services
{
    public class RabbitMqConsume : IRabbitMQConsume
    {
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;
        protected readonly IServiceProvider _serviceProvider;
        private readonly IRedisServices _redisServices;
        private readonly IMongoCollection<TimelineTweets> _timelineCollection;
        public RabbitMqConsume(IServiceProvider serviceProvider,
            IOptions<DatabaseSetting.DatabaseSetting> DBsetting,
            IRedisServices redisServices
            )
        {
            _factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://uslpaenl:EhK787ZeOdfT8Cerm4svZN2p53pD0mtl@beaver.rmq.cloudamqp.com/uslpaenl")

            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceProvider = serviceProvider;
            var client = new MongoClient(DBsetting.Value.connectionString);
            var db = client.GetDatabase(DBsetting.Value.databaseName);
            _timelineCollection = db.GetCollection<TimelineTweets>(DBsetting.Value.userTimelineCollection);
            _redisServices = redisServices;
        }

        public async Task Connect(string userName)
        {

            var followers = new List<string>();


            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (m, e) =>
            {
                byte[] body = e.Body.ToArray();
                var msg = (Tweet)JsonConvert.DeserializeObject<Tweet>(Encoding.UTF8.GetString(body))!;
                var collections = await _timelineCollection.Find(x => x.userName == userName).FirstOrDefaultAsync();
                collections.tweets.Insert(0, msg);
                await _timelineCollection.ReplaceOneAsync(x => x.userName == userName, collections);
                await _redisServices.SetCacheValueAsync(userName);
            };

            _channel.BasicConsume(queue: "Dopamine:" + userName,
                autoAck: true,
                consumer: consumer);
        }
    }
}
