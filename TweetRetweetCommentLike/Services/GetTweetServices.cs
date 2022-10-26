namespace TweetRetweetCommentLike.Services
{
    public class GetTweetServices : IGetTweetServices
    {
        private readonly IMongoCollection<TimelineTweets> _timelineCollection;
        public GetTweetServices(IOptions<DatabaseSetting.DatabaseSetting> db)
        {
            var client = new MongoClient(db.Value.connectionString);
            var database = client.GetDatabase(db.Value.databaseName);
            _timelineCollection = database.GetCollection<TimelineTweets>(db.Value.userTimelineCollection);
        }
        //get tweets
        public async Task<List<Tweet>> GetTimelineTweets(string userName)
        {
            var collections = await _timelineCollection.Find(x => x.userName == userName).FirstOrDefaultAsync();
            return collections.tweets;
        }


    }
}
