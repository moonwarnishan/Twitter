using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class TimelineTweetsControllers : ControllerBase
    {
        public readonly IGetTweetServices _GetTweetServices;
        public TimelineTweetsControllers(IGetTweetServices getTweetServices, IRedisServices redisServices)
        {
            _GetTweetServices = getTweetServices;
        }
        //get tweets
        [HttpGet("{userName}")]
        public async Task<List<TweetDto>> getTweets(string userName)
        {
            var tweets=await _GetTweetServices.GetTimelineTweets(userName);
            if (tweets == null)
            {
                return null;
            }
            else
            {
                return tweets;
            }
        }

        [HttpGet("{userName}/{tweetId}")]
        public async Task<TweetDto> getSpecificTweet(string userName, string tweetId)
        {
            return await _GetTweetServices.getTweetbyId(userName, tweetId);
        }

        [HttpGet("{userName}")]
        
        public async Task<List<TweetDto>> getTweetsbyuserName(string userName)
        {
            var tweets = await _GetTweetServices.GeTweetsbyuserName(userName);
            if (tweets == null)
            {
                return null;
            }
            else
            {
                return tweets;
            }
        }


        [HttpGet("{userName}")]
        public async Task<List<TweetDto>> getTweetsRedis(string userName)
        {
            var tweets = await _GetTweetServices.GetTimelineTweetsRedis(userName);
            if (tweets == null)
            {
                return null;
            }
            else
            {
                return tweets;
            }
        }


    }
}
