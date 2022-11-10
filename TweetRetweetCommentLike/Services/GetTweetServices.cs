namespace TweetRetweetCommentLike.Services
{
    public class GetTweetServices : IGetTweetServices
    {
        private readonly IMongoCollection<TimelineTweets> _timelineCollection;
        private readonly ILikeCommentRetweetServices _likeCommentRetweetServices;
        public readonly IRedisServices _RedisServices;

        public GetTweetServices(IOptions<DatabaseSetting.DatabaseSetting> db ,ILikeCommentRetweetServices likeCommentRetweetServices, IRedisServices redisServices)
        {
            var client = new MongoClient(db.Value.connectionString);
            var database = client.GetDatabase(db.Value.databaseName);
            _timelineCollection = database.GetCollection<TimelineTweets>(db.Value.userTimelineCollection);
            _likeCommentRetweetServices = likeCommentRetweetServices;
            _RedisServices = redisServices;
        }
        //get tweets
        public async Task<List<TweetDto>> GetTimelineTweets(string userName,int page)
        {
            var collections = await _timelineCollection.Find(x => x.userName == userName).FirstOrDefaultAsync();
            var Tweets=new List<TweetDto>();
            var twts=collections.tweets.Take(page*10).ToList();

            foreach (var timelineTweet in twts)
            {
                var likecmnts = await _likeCommentRetweetServices.getAll(timelineTweet.tweetId);
                if (likecmnts != null)
                {
                    var dto = new TweetDto()
                    {
                        tweetId = timelineTweet.tweetId,
                        userName = timelineTweet.userName,
                        tweetText = timelineTweet.tweetText,
                        tweetTime = timelineTweet.tweetTime,
                        comments = likecmnts.comments,
                        likes = likecmnts.likes,
                        retweets = likecmnts.retweets
                    };
                    Tweets.Add(dto);
                }
                else
                {
                    var dto = new TweetDto()
                    {
                        tweetId = timelineTweet.tweetId,
                        userName = timelineTweet.userName,
                        tweetText = timelineTweet.tweetText,
                        tweetTime = timelineTweet.tweetTime,
                        comments = new List<CommentDto>(),
                        likes = new List<string>(),
                        retweets = new List<RetweetDto>()
                    };
                    Tweets.Add(dto);
                }
                
            }

            return Tweets;
        }

        //get specific tweet
        public async Task<TweetDto> getTweetbyId(string userName, string TweetId)
        {
            var tweet =await  _timelineCollection.Find(x => x.userName == userName).FirstOrDefaultAsync();
            var twt = tweet.tweets.Find(x => x.tweetId == TweetId);
            var likecmnts =await _likeCommentRetweetServices.getAll(TweetId);
            if (likecmnts != null)
            {
                var dto = new TweetDto()
                {
                    tweetId = TweetId,
                    userName =twt.userName,
                    tweetText = twt.tweetText,
                    tweetTime = twt.tweetTime,
                    comments = likecmnts.comments,
                    likes = likecmnts.likes,
                    retweets = likecmnts.retweets
                };
                return dto;
            }
            else
            {
                var dto = new TweetDto()
                {
                    tweetId = twt.tweetId,
                    userName = twt.userName,
                    tweetText = twt.tweetText,
                    tweetTime = twt.tweetTime,
                    comments = new List<CommentDto>(),
                    likes = new List<string>(),
                    retweets = new List<RetweetDto>()
                };
                return dto;
            }

        }

        public async Task<List<TweetDto>> GeTweetsbyuserName(string userName,int page)
        {
            var collection = await _timelineCollection.Find(x => x.userName == userName).FirstOrDefaultAsync();

            var tweets = collection.tweets.FindAll(x => x.userName == userName);
            tweets = tweets.Take(page * 5).ToList();
            var Tweets = new List<TweetDto>();

            foreach (var timelineTweet in tweets)
            {
                var likecmnts = await _likeCommentRetweetServices.getAll(timelineTweet.tweetId);
                if (likecmnts != null)
                {
                    var dto = new TweetDto()
                    {
                        tweetId = timelineTweet.tweetId,
                        userName = timelineTweet.userName,
                        tweetText = timelineTweet.tweetText,
                        tweetTime = timelineTweet.tweetTime,
                        comments = likecmnts.comments,
                        likes = likecmnts.likes,
                        retweets = likecmnts.retweets
                    };
                    Tweets.Add(dto);
                }
                else
                {
                    var dto = new TweetDto()
                    {
                        tweetId = timelineTweet.tweetId,
                        userName = timelineTweet.userName,
                        tweetText = timelineTweet.tweetText,
                        tweetTime = timelineTweet.tweetTime,
                        comments = new List<CommentDto>(),
                        likes = new List<string>(),
                        retweets = new List<RetweetDto>()
                    };
                    Tweets.Add(dto);
                }

            }

            return Tweets;
        }


        //Get Tweet From Redis
        public async Task<List<TweetDto>> GetTimelineTweetsRedis(string userName)
        {
            var collections =await _RedisServices.timelineTweetsFromRedis(userName);

            if (collections != null)
            {
                var Tweets = new List<TweetDto>();

                foreach (var timelineTweet in collections)
                {
                    var likecmnts = await _likeCommentRetweetServices.getAll(timelineTweet.tweetId);
                    if (likecmnts != null)
                    {
                        var dto = new TweetDto()
                        {
                            tweetId = timelineTweet.tweetId,
                            userName = timelineTweet.userName,
                            tweetText = timelineTweet.tweetText,
                            tweetTime = timelineTweet.tweetTime,
                            comments = likecmnts.comments,
                            likes = likecmnts.likes,
                            retweets = likecmnts.retweets
                        };
                        Tweets.Add(dto);
                    }
                    else
                    {
                        var dto = new TweetDto()
                        {
                            tweetId = timelineTweet.tweetId,
                            userName = timelineTweet.userName,
                            tweetText = timelineTweet.tweetText,
                            tweetTime = timelineTweet.tweetTime,
                            comments = new List<CommentDto>(),
                            likes = new List<string>(),
                            retweets = new List<RetweetDto>()
                        };
                        Tweets.Add(dto);
                    }

                }

                return Tweets;
            }

            return null;
        }

        //remove tweets
        public async Task RemoveTweet(string followedName, string userName)
        {
            var collections = await _timelineCollection.Find(x => x.userName == userName).FirstOrDefaultAsync();
            collections.tweets.RemoveAll(x => x.userName == followedName);
            await _timelineCollection.ReplaceOneAsync(x => x.userName == userName, collections);
        }


    }
}
