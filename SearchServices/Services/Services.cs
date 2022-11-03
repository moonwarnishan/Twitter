
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace SearchServices.Services
{
    public class Services : IServices
    {
        private IMongoCollection<UserResponseDto> _users;
        private IMongoCollection<TweetSearchDto> _tweets;
        public  Services(IOptions<DatabaseSetting.DatabaseSetting> dbSetting)
        {
            var client = new MongoClient(dbSetting.Value.connectionString);
            var db = client.GetDatabase(dbSetting.Value.databaseName);
            _users= db.GetCollection<UserResponseDto>(dbSetting.Value.userSearchCollection);
            _tweets = db.GetCollection<TweetSearchDto>(dbSetting.Value.hashSearchCollection);
        }


        public async Task<List<UserDto>> Users(string keyword)
        {
            var dtos=new List<UserDto>();
            var list = await _users.Find(x => true).ToListAsync();
            foreach (var l in list)
            {
                if (l._id.ToLower().Contains(keyword.ToLower()))
                {
                    var dto=new UserDto();
                    dto.userName = l._id.Split(':')[0];
                    dto.name=l._id.Split(':')[1];
                    dto.userId = l.value;
                    dtos.Add(dto);
                }
            }

            if (dtos.Count > 0)
            {
                return dtos;
            }

            return null;
        }

        public async Task<List<HashResponseDto>> Hashes(string keyword)
        {
            var list = await _tweets.Find(x => true).ToListAsync();
            var hashList=new List<HashResponseDto>();
            foreach (var dt in list)
            {
                if (dt.tweetText.ToLower().Contains(keyword))
                {
                    var hash = new HashResponseDto();
                    hash.userName = dt.userName;
                    hash.tweetId = dt.tweetId;
                    hash.tweetText = dt.tweetText;
                    hashList.Add(hash);
                }
            }

            if (hashList.Count > 0)
            {
                return hashList;
            }

            return null;
        }
    }
}
