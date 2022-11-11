using NReJSON;

namespace TweetRetweetCommentLike.Services
{
    public class RedisServices : IRedisServices
    {
        private readonly IConnectionMultiplexer _connection;
        public RedisServices(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public async Task<List<TweetDto>> timelineTweetsFromRedis(string userName)
        {
            try
            {
                var dtos = new List<TweetDto>();
                var db = _connection.GetDatabase();
                var key = "Dopamine_" + userName;
                var tweetsJson =await db.StringGetAsync(key);
                var tweets = JsonConvert.DeserializeObject<List<TweetDto>>(tweetsJson.ToString());
                return tweets;
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

    }
}
