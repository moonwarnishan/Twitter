namespace RegisterUser.Services
{
    public class SearchServiceMongo  : ISearchServiceMongo
    {
        private readonly IMongoCollection<UserSearch> _usersSearchCollection;

        public SearchServiceMongo(IOptions<DatabaseSetting.DatabaseSetting> DBsetting)
        {
            var client = new MongoClient(DBsetting.Value.connectionString);
            var db = client.GetDatabase(DBsetting.Value.databaseName);
            _usersSearchCollection = db.GetCollection<UserSearch>(DBsetting.Value.userSearchCollection);
        }

        //add new
        public async Task createNewSearch(UserSearch userSearch)
        {
            await _usersSearchCollection.InsertOneAsync(userSearch);
        }

    }
}
