namespace TweetRetweetCommentLike.Services
{
    public class FollowBlockServices : IFollowBlockServices
    {
        private readonly IMongoCollection<FollowAndBlock> _followAndBlock;
        private readonly IFollowBlockIndividualServices _followblockServices;
        public FollowBlockServices(IOptions<DatabaseSetting.DatabaseSetting> DBsetting, IFollowBlockIndividualServices followblockServices)
        {
            var client = new MongoClient(DBsetting.Value.connectionString);
            var db = client.GetDatabase(DBsetting.Value.databaseName);
            _followAndBlock = db.GetCollection<FollowAndBlock>(DBsetting.Value.followBlockCollectionName);
            _followblockServices = followblockServices;
        }
        //create new follow
        public async Task Create(string folloeName, string followerName)
        {
            var follower = await _followAndBlock.Find(f => f.folloeName == folloeName).FirstOrDefaultAsync();
            if (follower == null)
            {
                await _followAndBlock.InsertOneAsync(new FollowAndBlock { folloeName = folloeName, followerNames = new List<string> { followerName } });
                await _followblockServices.CreateFollowee(followerName, folloeName);
            }
            else
            {
                follower.followerNames.Add(followerName);
                await _followAndBlock.ReplaceOneAsync(f => f.folloeName == folloeName, follower);
                await _followblockServices.CreateFollowee(followerName, folloeName);
            }
        }
        //delete follow
        public async Task Delete(string folloeName, string followerName)
        {
            var follower = await _followAndBlock.Find(f => f.folloeName == folloeName).FirstOrDefaultAsync();
            if (follower != null)
            {
                follower.followerNames.Remove(followerName);
                await _followAndBlock.ReplaceOneAsync(f => f.folloeName == folloeName, follower);
          
            }
        }
        //get all followers
        public async Task<List<string>> GetFollowedUsers(string folloeName)
        {
            var follower = await _followAndBlock.Find(f => f.folloeName == folloeName).FirstOrDefaultAsync();
            if (follower != null)
            {
                return follower.followerNames;
            }
            return null;
        }
        //followed or not
        public async Task<bool> IsFollowed(string followerName,string folloeName)
        {
            var follower = await _followAndBlock.Find(f => f.folloeName == folloeName).FirstOrDefaultAsync();
            if (follower != null)
            {
                return follower.followerNames.Contains(followerName);
            }
            return false;
        }
        //create block
        public async Task CreateBlock(string blockUserName, string blockedUserName)
        {
            var blockedUser = await _followAndBlock.Find(b => b.folloeName== blockUserName).FirstOrDefaultAsync();
            if (blockedUser == null)
            {
                await _followAndBlock.InsertOneAsync(new FollowAndBlock { folloeName = blockUserName, blockedNames  = new List<string> { blockedUserName } });
                await _followblockServices.CreateBlockee(blockUserName, blockedUserName);
            }
            else
            {
                blockedUser.blockedNames.Add(blockedUserName);
                await _followAndBlock.ReplaceOneAsync(b => b.folloeName == blockUserName, blockedUser);
                await _followblockServices.CreateBlockee(blockUserName, blockedUserName);
            }
        }
        //delete block
        public async Task DeleteBlock(string blockUserName, string blockedUserName)
        {
            var blockedUser = await _followAndBlock.Find(b => b.folloeName == blockUserName).FirstOrDefaultAsync();
            if (blockedUser != null)
            {
                blockedUser.blockedNames.Remove(blockedUserName);
                await _followblockServices.DeleteBlockee(blockUserName, blockedUserName);
                await _followAndBlock.ReplaceOneAsync(b => b.folloeName == blockUserName, blockedUser);
            }
        }
        //get all block
        public async Task<List<string>> GetBlockedUsers(string blockUserName)
        {
            var blockedUser = await _followAndBlock.Find(b => b.folloeName == blockUserName).FirstOrDefaultAsync();
            if (blockedUser != null)
            {
                return blockedUser.blockedNames;
            }
            return null;
        }
        //blocked or not
        public async Task<bool> IsBlocked(string blockUserName, string blockedUserName)
        {
            var blockedUser = await _followAndBlock.Find(b => b.folloeName == blockUserName).FirstOrDefaultAsync();
            bool blockedByOther = await _followblockServices.IsBlockee( blockedUserName,blockUserName);
            if (blockedUser != null)
            {
                return blockedUser.blockedNames.Contains(blockedUserName);
            }
            return false;
        }

        //number of followings
        public async Task<int> GetNumberOfFollowings(string folloeName)
        {
            var follower = await _followAndBlock.Find(f => f.folloeName == folloeName).FirstOrDefaultAsync();
            if (follower != null)
            {
                return follower.followerNames.Count;
            }
            return 0;
        }


    }
}