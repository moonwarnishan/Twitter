namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LikeCommentRetweetController : ControllerBase
    {
        private readonly ILikeCommentRetweetServices _likeCommentRetweetServices;
        private readonly ILogger<LikeCommentRetweetController> _logger;
        public LikeCommentRetweetController(ILikeCommentRetweetServices likeCommentRetweetServices, ILogger<LikeCommentRetweetController> logger)
        {
            _likeCommentRetweetServices = likeCommentRetweetServices;
            _logger = logger;
        }

        //createordeletelike
        [HttpPut("{tweetId}/{receiverUserName}/{userName}")]
        public IActionResult CreateOrDeleteLike(string tweetId, string receiverUserName, string userName)
        {
            try
            {
                var result = _likeCommentRetweetServices.CreateOrDeleteLike(tweetId, receiverUserName, userName);
                if (result == null)
                {
                    _logger.LogWarning("result is null");
                    return NotFound();
                }
                _logger.LogInformation("User {0} like/unlike tweet {1}", userName, tweetId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        //createordeleteretweet
        [HttpPut("{tweetId}/{receiverUserName}/{userName}")]
        public IActionResult CreateOrDeleteReTweet(string tweetId,string receiverUserName, RetweetDto retweet)
        {
            try
            {
                var result = _likeCommentRetweetServices.CreateOrDeleteReTweet(tweetId, receiverUserName, retweet);
                if (result == null)
                {
                    _logger.LogWarning("result is null");
                    return NotFound();
                }
                _logger.LogInformation("User {0} retweet/unretweet tweet {1}", retweet.userName, tweetId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        //createcomment
        [HttpPut("{tweetId}/{receiverUserName}/{userName}")]
        public IActionResult CreateComment(string tweetId,string  receiverUserName, CommentDto comment)
        {
            try
            {
                var result =  _likeCommentRetweetServices.CreateComment(tweetId, receiverUserName, comment);
                if (result == null)
                {
                    _logger.LogWarning("result is null");
                    return NotFound();
                }
                _logger.LogInformation("User {0} comment tweet {1}", comment.userName, tweetId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        //deletecomment
        [HttpDelete]
        [HttpPut("{tweetId}/{commentId}/{userName}")]
        public IActionResult DeleteComment(string tweetId, string commentId,string userName)
        {
            try
            {
                var result = _likeCommentRetweetServices.DeleteComment(tweetId, commentId,userName);
                if (result == null)
                {
                    _logger.LogWarning("result is null");
                    return NotFound();
                }
                _logger.LogInformation("User {0} delete comment {1} from tweet {2}", userName, commentId, tweetId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Bad Request");
                return BadRequest();
            }
        }
        [HttpGet("{tweetId}")]
        public async Task<CommentLikeRetweet> getLikecommentRetweet(string tweetId)
        {
            return await _likeCommentRetweetServices.getAll(tweetId);
        }

    }
}
