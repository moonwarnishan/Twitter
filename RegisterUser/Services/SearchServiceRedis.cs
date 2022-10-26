

namespace RegisterUser.Services
{
    public class SearchServiceRedis : ISearchServiceRedis
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly SearchServiceMongo _searchServiceMongo;

        public SearchServiceRedis(IConnectionMultiplexer connection, SearchServiceMongo searchServiceMongo)
        {
            _connection = connection;
            _searchServiceMongo = searchServiceMongo;
        }
        
        public async Task SetUserSearchValue(string userName,string name, string userId)
        {
            var db = _connection.GetDatabase();
            var searchValue = new UserSearch
            {
                key= "Dopamine_User_Search:" + userName + "+" + name,
                value = userId
            };
            await _searchServiceMongo.createNewSearch(searchValue);
            await db.StringSetAsync("Dopamine_User_Search:" + userName+"+"+name, userId);
        }

        public async Task DeleteKey(string userName, string name)
        {
            var db= _connection.GetDatabase();
            await db.KeyDeleteAsync("Dopamine_User_Search:" + userName + "+" + name);
        }

    }
}
