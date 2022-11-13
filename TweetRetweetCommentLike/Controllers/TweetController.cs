namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TweetController : ControllerBase
    {
        private readonly ITweetServices _tweetServices;
        private readonly ILogger<TweetController> _logger;
        public TweetController(ITweetServices tweetServices, ILogger<TweetController> logger)
        {
            _tweetServices = tweetServices;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> createTweet(Tweet tweet)
        {
            await _tweetServices.createTweet(tweet);
            _logger.LogInformation("Tweet created by {0}", tweet.userName);
            return Ok();
        }
        [HttpDelete("{userName}/{tweetId}")]
        public async Task<ActionResult> DeleteTweet(string userName,string tweetId)
        {
            await _tweetServices.DeleteTweet(userName,tweetId);
            _logger.LogInformation("Tweet deleted by {0}", userName);
            return Ok();
        }
    }
}
