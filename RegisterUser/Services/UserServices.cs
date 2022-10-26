namespace RegisterUser.Services
{
    public class UserServices : IUserServices
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<TimelineTweets> _timelCollection;
        public UserServices(IOptions<DatabaseSetting.DatabaseSetting> DBsetting)
        {
            var client = new MongoClient(DBsetting.Value.connectionString);
            var db = client.GetDatabase(DBsetting.Value.databaseName);
            _usersCollection = db.GetCollection<User>(DBsetting.Value.usersCollectionName);
            _timelCollection = db.GetCollection<TimelineTweets>(DBsetting.Value.userTimelineCollection);

        }
        public async Task CreateAsync(User newUser)
        {
            var timelineTweet = new TimelineTweets()
            {
                userName = newUser.userName,
                tweets = new List<Tweet>()
            };
            await _timelCollection.InsertOneAsync(timelineTweet);
            await _usersCollection.InsertOneAsync(newUser);
        }

  
        public async Task<User?> GetUserByIdAsync(string id) =>
            await _usersCollection.Find(x => x.userId == id).FirstOrDefaultAsync();


        public async Task UpdateAsync(User updatedUser) =>
            await _usersCollection.ReplaceOneAsync(x => x.userId == updatedUser.userId, updatedUser);
        

        public async Task<List<User>> GetAllUsersAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();
        
        public IMongoCollection<User> Users()
        {
            return _usersCollection;
        }
        
        public async Task<User> FindByuserNameAsync(string userName) =>
            await _usersCollection.Find(x => x.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();

        public User LoginValidation(LoginModel M) =>
            _usersCollection.Find(x => x.userName.ToLower() == M.userName.ToLower() && x.password == PasswordHash.HashPassword(M.password)).FirstOrDefault();

        public User FindByuserName(string userName) =>
             _usersCollection.Find(x => x.userName.ToLower() == userName.ToLower()).FirstOrDefault();
        //find user by email
        public User FindByEmail(string email) =>
            _usersCollection.Find(x => x.email.ToLower() == email.ToLower()).FirstOrDefault();
    }
}
