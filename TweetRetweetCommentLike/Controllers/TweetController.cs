namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TweetController : ControllerBase
    {
        private readonly ITweetServices _tweetServices;
        public TweetController(ITweetServices tweetServices)
        {
            _tweetServices = tweetServices;
        }
        [HttpPost]
        public async Task<ActionResult> createTweet(Tweet tweet)
        {
            await _tweetServices.createTweet(tweet);
            return Ok();
        }
        [HttpDelete("{userName}/{tweetId}")]
        public async Task<ActionResult> DeleteTweet(string userName,string tweetId)
        {
            await _tweetServices.DeleteTweet(userName,tweetId);
            return Ok();
        }
    }
}
