
namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FollowBlockController : ControllerBase
    {
        private readonly IFollowBlockServices _followBlockService;
        private readonly IFollowBlockIndividualServices _followBlockIndividualService;
        public FollowBlockController(IFollowBlockServices followService, IFollowBlockIndividualServices followBlockIndividualService)
        {
            _followBlockService = followService;
            _followBlockIndividualService = followBlockIndividualService;
        }
        
        //new follow
        [HttpPost("{followUserName}/{followedUserName}")]
        public async Task<ActionResult> Create(string followUserName, string followedUserName)
        {
            bool check = await _followBlockService.IsBlocked(followUserName, followedUserName);
            if (check)
            {
                return BadRequest("You are blocked this user so you can not follow him");
            }
            await _followBlockService.Create(followUserName, followedUserName);
            return Ok();
        }
        //delete follow
        [HttpDelete("{followUserName}/{followedUserName}")]
        public async Task<ActionResult> Delete(string followUserName, string followedUserName)
        {
            await _followBlockService.Delete(followUserName, followedUserName);
            await _followBlockIndividualService.DeleteFollowee(followedUserName,followUserName);
            return Ok();
        }
        //get all followed users
        [HttpGet("{followUserName}")]

        public async Task<ActionResult<List<string>>> GetFollowedUsers(string followUserName)
        {
            var followedUsers = await _followBlockService.GetFollowedUsers(followUserName);
            return Ok(followedUsers);
        }
        //get all followers
        [HttpGet("{followingUserName}")]

        public async Task<List<string>> GetFollowerUsers(string followingUserName)
        {
            var followerUsers = await _followBlockIndividualService.GetAllFollowers(followingUserName);
            return followerUsers;
        }

        //a user followed or not
        [HttpGet("{followedUserName}/{followUserName}")]

        public async Task<ActionResult<bool>> IsFollowed(string followUserName, string followedUserName)
        {
            var isFollowed = await _followBlockService.IsFollowed(followedUserName,followUserName);
            return Ok(isFollowed);
        }
        //block a user
        [HttpPost("{userName}/{blockedUserName}")]

        public async Task<ActionResult> Block(string userName, string blockedUserName)
        {
            await _followBlockService.Delete(blockedUserName,userName);
            await _followBlockIndividualService.DeleteFollowee(userName,blockedUserName);
            await _followBlockService.CreateBlock(userName, blockedUserName);
            return Ok();
        }
        //unblock a user
        [HttpDelete("{userName}/{blockedUserName}")]

        public async Task<ActionResult> Unblock(string userName, string blockedUserName)
        {
            await _followBlockService.DeleteBlock(userName, blockedUserName);
            return Ok();
        }
        //get all blocked users
        [HttpGet("{userName}")]

        public async Task<ActionResult<List<string>>> GetBlockedUsers(string userName)
        {
            var blockedUsers = await _followBlockService.GetBlockedUsers(userName);
            return Ok(blockedUsers);
        }
        //is blocked
        [HttpGet("{userName}/{blockedUserName}")]
        public async Task<ActionResult<bool>> IsBlocked(string userName, string blockedUserName)
        {
            var isBlocked = await _followBlockService.IsBlocked(userName, blockedUserName);
            var isBlocked2=  await _followBlockIndividualService.IsBlockee(userName, blockedUserName);
            return Ok(isBlocked || isBlocked2);
        }
        //follower and following count
        [HttpGet("{userName}")]
        public async Task<FollowFollowingDto> GetFollowFollowingCount(string userName)
        {
            var follower=await _followBlockIndividualService.NumberOfFollowers(userName);
            var following = await _followBlockService.GetNumberOfFollowings(userName);
            var response = new FollowFollowingDto()
            {
                follower = follower,
                following = following
            };
            return response;
        }

    }
}
