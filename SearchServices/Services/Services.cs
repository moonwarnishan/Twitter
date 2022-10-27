
using MongoDB.Driver;

namespace SearchServices.Services
{
    public class Services : IServices
    {
        private IMongoCollection<UserResponseDto> _users;
        private IMongoCollection<HashResponseDto> _tweets;
        public  Services(IOptions<DatabaseSetting.DatabaseSetting> dbSetting)
        {
            var client = new MongoClient(dbSetting.Value.connectionString);
            var db = client.GetDatabase(dbSetting.Value.databaseName);
            _users= db.GetCollection<UserResponseDto>(dbSetting.Value.userSearchCollection);
            _tweets = db.GetCollection<HashResponseDto>(dbSetting.Value.hashSearchCollection);
        }


        public Task<List<UserResponseDto>> Users(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task<List<HashResponseDto>> Hashes(string keyword)
        {
            throw new NotImplementedException();
        }
    }
}
