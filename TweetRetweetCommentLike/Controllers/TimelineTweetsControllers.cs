using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TimelineTweetsControllers : ControllerBase
    {
        public readonly IGetTweetServices _GetTweetServices;
        public TimelineTweetsControllers(IGetTweetServices getTweetServices)
        {
            _GetTweetServices = getTweetServices;
        }
        //get tweets
        [HttpGet("{userName}")]
        public async Task<List<Tweet>> getTweets(string userName)
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


    }
}
