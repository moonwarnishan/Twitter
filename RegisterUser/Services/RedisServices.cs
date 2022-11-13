namespace RegisterUser.Services
{
    public class RedisServices : IRedisServices
    {
        private readonly IMongoCollection<TimelineTweets> _timelineCollection;
        private readonly IConnectionMultiplexer _connection;
        public RedisServices(IOptions<DatabaseSetting.DatabaseSetting> DBsetting, IConnectionMultiplexer connection)
        {
            var client = new MongoClient(DBsetting.Value.connectionString);
            var db = client.GetDatabase(DBsetting.Value.databaseName);
            _timelineCollection = db.GetCollection<TimelineTweets>(DBsetting.Value.userTimelineCollection);
            _connection = connection;
        }

        public async Task SetCacheValueAsync(string Name)
        {

            var db = _connection.GetDatabase();
            var key = "Dopamine_" + Name;
            var collection = await _timelineCollection.Find(x => x.userName == Name).FirstOrDefaultAsync();
            
            if (collection.tweets.Count >= 10)
            {
                var timelineTweets = JsonConvert.SerializeObject(collection.tweets.Take(10));
                await db.StringSetAsync(key, timelineTweets, TimeSpan.FromHours(4));
            }
            else
            {
                var timelineTweets = JsonConvert.SerializeObject(collection.tweets);
                await db.StringSetAsync(key, timelineTweets, TimeSpan.FromHours(2));
            }
            
        }



    }
}
