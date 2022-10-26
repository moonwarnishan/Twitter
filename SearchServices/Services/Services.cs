
namespace SearchServices.Services
{
    public class Services : IServices
    {
        private readonly IConnectionMultiplexer _connection;
        public Services(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public async Task<List<UserResponseDto>> Users(string keyword)
        {
            var db = _connection.GetServer("127.0.0.1:6379");

            var user = new List<UserResponseDto>();
            RedisKey[] UsersHashs = db.Keys(pattern: "Dopamine_User_Search:*").ToArray();
            foreach (string u in UsersHashs)
            {
                if (u.Split(":")[1].ToLower().Contains(keyword.ToLower()))
                {

                    var usr = new UserResponseDto()
                    {
                        name = u.Split(":")[1].Split("+")[1],
                        userName = u.Split(":")[1].Split("+")[0]
                    };
                    user?.Add(usr);
                }

            }

            return user;

        }

        public async Task<List<HashResponseDto>> Hashes(string keyword)
        {
            var db = _connection.GetServer("127.0.0.1:6379");
            var db2 = _connection.GetDatabase();
            var tweets = new List<HashResponseDto>();
            RedisKey[] TweetsHashs =db.Keys(pattern: "Dopamine_Hash_Search:*").ToArray();
            foreach (string t in TweetsHashs)
            {

                if (t.Split(":")[1].Contains(keyword))
                {
                    
                    var h = new HashResponseDto()
                    {
                        tweetId = db2.StringGet(t),
                        tweetText = t.Split(":")[1].Split("+")[1],
                        userName = t.Split(":")[1].Split("+")[2]
                    };
                    tweets.Add(h);
                }

            }

            return tweets;
        }

       
    }
}
