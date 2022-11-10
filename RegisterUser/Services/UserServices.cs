namespace RegisterUser.Services
{
    public class UserServices : IUserServices
    {
        private readonly IMongoCollection<UserInfo> _usersCollection;
        private readonly IMongoCollection<TimelineTweets> _timelCollection;
        private readonly ISearchServiceMongo _searchService;
        public UserServices(IOptions<DatabaseSetting.DatabaseSetting> DBsetting,ISearchServiceMongo searchService)
        {
            var client = new MongoClient(DBsetting.Value.connectionString);
            var db = client.GetDatabase(DBsetting.Value.databaseName);
            _usersCollection = db.GetCollection<UserInfo>(DBsetting.Value.usersCollectionName);
            _timelCollection = db.GetCollection<TimelineTweets>(DBsetting.Value.userTimelineCollection);
            _searchService = searchService;
        }
        public async Task CreateAsync(UserInfo newUser)
        {
            var timelineTweet = new TimelineTweets()
            {
                userName = newUser.userName,
                tweets = new List<Tweet>()
            };
            var userSearch = new UserSearch()
            {
                key = newUser.userName+':'+newUser.name+':'+newUser.email,
                value = newUser.userId
            };
            await _searchService.createNewSearch(userSearch);
            await _timelCollection.InsertOneAsync(timelineTweet);
            await _usersCollection.InsertOneAsync(newUser);
        }

  
        public async Task<UserInfo?> GetUserByIdAsync(string id) =>
            await _usersCollection.Find(x => x.userId == id).FirstOrDefaultAsync();


        public async Task UpdateAsync(UserInfo updatedUser) =>
            await _usersCollection.ReplaceOneAsync(x => x.userId == updatedUser.userId, updatedUser);
        

        public async Task<List<UserInfo>> GetAllUsersAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();
        
        public IMongoCollection<UserInfo> Users()
        {
            return _usersCollection;
        }
        
        public async Task<UserInfo> FindByuserNameAsync(string userName) =>
            await _usersCollection.Find(x => x.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
        public UserInfo LoginValidation(LoginModel M) =>
            _usersCollection.Find(x => x.userName.ToLower() == M.userName.ToLower() && x.password == PasswordHash.HashPassword(M.password)).FirstOrDefault();

        public UserInfo FindByuserName(string userName) =>
             _usersCollection.Find(x => x.userName.ToLower() == userName.ToLower()).FirstOrDefault();
        //find user by email
        public UserInfo FindByEmail(string email) =>
            _usersCollection.Find(x => x.email.ToLower() == email.ToLower()).FirstOrDefault();
    }
}
