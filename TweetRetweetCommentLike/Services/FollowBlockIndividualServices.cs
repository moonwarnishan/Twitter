namespace TweetRetweetCommentLike.Services
{
    public class FollowBlockIndividualServices : IFollowBlockIndividualServices
    {
        private readonly IMongoCollection<FollowBlockIndividual> _folllowees;
        private readonly IGetTweetServices _getTweetServices;

        public FollowBlockIndividualServices(IOptions<DatabaseSetting.DatabaseSetting> DBsetting, IGetTweetServices getTweetServices)
        {
            var client = new MongoClient(DBsetting.Value.connectionString);
            var db = client.GetDatabase(DBsetting.Value.databaseName);
            _folllowees = db.GetCollection<FollowBlockIndividual>(DBsetting.Value.followeeBlockeeCollectionName);
            _getTweetServices = getTweetServices;
        }
        //create new follow
        public async Task CreateFollowee(string userName, string followeeName)
        {
            var followee = await _folllowees.Find(f => f.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            if (followee == null)
            {
                await _folllowees.InsertOneAsync(new FollowBlockIndividual { userName = userName, FollowedByUsersNames = new List<string> { followeeName } });
            }
            else
            {
                followee.FollowedByUsersNames.Add(followeeName);
                await _folllowees.ReplaceOneAsync(f => f.userName == userName, followee);
            }
        }
        //delete follow
        public async Task DeleteFollowee(string userName, string followeeName)
        {
            var followee = await _folllowees.Find(f => f.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            if (followee != null)
            {
                await _getTweetServices.RemoveTweet(userName, followeeName);
                followee.FollowedByUsersNames.Remove(followeeName);
                await _folllowees.ReplaceOneAsync(f => f.userName == userName, followee);
            }
        }
        //get all followers
        public async Task<List<string>> GetFollowees(string userName)
        {
            var followee = await _folllowees.Find(f => f.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            if (followee != null)
            {
                return followee.FollowedByUsersNames;
            }
            return null;
        }
        //isfollow
        public async Task<bool> IsFollowee(string userName, string followeeName)
        {
            var followee = await _folllowees.Find(f => f.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            if (followee != null)
            {
                return followee.FollowedByUsersNames.Contains(followeeName);
            }
            return false;
        }

        //create new block
        public async Task CreateBlockee(string userName, string blockeeName)
        {
            var blockee = await _folllowees.Find(f => f.userName.ToLower() == blockeeName.ToLower()).FirstOrDefaultAsync();
            if (blockee == null)
            {
                await _getTweetServices.RemoveTweet(userName, blockeeName);
                await _getTweetServices.RemoveTweet(blockeeName, userName);
                await _folllowees.InsertOneAsync(new FollowBlockIndividual { userName = blockeeName, blockedByUsersNames  = new List<string> { userName } });
            }
            else
            {
                await _getTweetServices.RemoveTweet(userName, blockeeName);
                await _getTweetServices.RemoveTweet(blockeeName, userName);
                blockee.blockedByUsersNames.Add(userName);
                await _folllowees.ReplaceOneAsync(f => f.userName ==blockeeName, blockee);
            }
        }
        //delete block
        public async Task DeleteBlockee(string userName, string blockeeName)
        {
            var blockee = await _folllowees.Find(f => f.userName.ToLower() == blockeeName.ToLower()).FirstOrDefaultAsync();
            if (blockee != null)
            {
                blockee.blockedByUsersNames.Remove(userName);
                await _folllowees.ReplaceOneAsync(f => f.userName == blockeeName, blockee);
            }
        }
        //get all blockees
        public async Task<List<string>> GetBlockees(string userName)
        {
            var blockee = await _folllowees.Find(f => f.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            if (blockee != null)
            {
                return blockee.blockedByUsersNames;
            }
            return null;
        }
        //is block
        public async Task<bool> IsBlockee(string userName, string blockeeName)
        {
            var blockee = await _folllowees.Find(f => f.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            if (blockee != null)
            {
                return blockee.blockedByUsersNames.Contains(blockeeName);
            }
            return false;
        }
        //number of followers
        public async Task<int> NumberOfFollowers(string userName)
        {
            var followee = await _folllowees.Find(f => f.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            if (followee != null)
            {
                return followee.FollowedByUsersNames.Count;
            }
            return 0;
        }
        //get all followers
        public async Task<List<string>> GetAllFollowers(string userName)
        {
            var followee = await _folllowees.Find(f => f.userName.ToLower() == userName.ToLower()).FirstOrDefaultAsync();
            if (followee != null)
            {
                return followee.FollowedByUsersNames;
            }
            return null;
        }


    }
}
