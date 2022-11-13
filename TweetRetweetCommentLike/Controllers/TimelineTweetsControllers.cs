namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TimelineTweetsControllers : ControllerBase
    {
        public readonly IGetTweetServices _GetTweetServices;
        private readonly ILogger<TimelineTweetsControllers> _logger;
        public TimelineTweetsControllers(IGetTweetServices getTweetServices, IRedisServices redisServices, ILogger<TimelineTweetsControllers> logger)
        {
            _GetTweetServices = getTweetServices;
            _logger = logger;
        }
        //get tweets
        [HttpGet("{userName}/{page}")]
        public async Task<List<TweetDto>> getTweets(string userName, int page)
        {
            var tweets=await _GetTweetServices.GetTimelineTweets(userName,page);
            if (tweets == null)
            {
                _logger.LogWarning("No tweets found for {0}", userName);
                return null;
            }
            else
            {
                _logger.LogInformation("Get tweets of {0}", userName);
                return tweets;
            }
        }

        [HttpGet("{userName}/{tweetId}")]
        public async Task<TweetDto> getSpecificTweet(string userName, string tweetId)
        {
            _logger.LogInformation("Get tweet of {0} tweetId is {1}", userName, tweetId);
            return await _GetTweetServices.getTweetbyId(userName, tweetId);
        }

        [HttpGet("{userName}/{page}")]
        
        public async Task<List<TweetDto>> getTweetsbyuserName(string userName, int page)
        {
            var tweets = await _GetTweetServices.GeTweetsbyuserName(userName,page);
            if (tweets == null)
            {
                _logger.LogWarning("tweet  is null for {0}", userName);
                return null;
            }
            else
            {
                _logger.LogInformation("Get All tweets of {0}", userName);
                return tweets;
            }
        }


        [HttpGet("{userName}")]
        public async Task<List<TweetDto>> getTweetsRedis(string userName)
        {
            var tweets = await _GetTweetServices.GetTimelineTweetsRedis(userName);
            if (tweets == null)
            {
                _logger.LogWarning("Redis doesnot find any tweet");
                return null;
            }
            else
            {
                _logger.LogInformation("Get All tweets from redis of {0}", userName);
                return tweets;
            }
        }


    }
}
