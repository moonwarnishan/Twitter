
namespace RegisterUser.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly UserServices _userServices;
        private readonly ISearchServiceMongo _searchServiceMongo;
        private readonly ILogger<UserController> _logger;

        public UserController(UserServices userServices, ISearchServiceMongo searchServiceMongo, ILogger<UserController> logger)
        {
            _userServices = userServices;
            _searchServiceMongo = searchServiceMongo;
            _logger = logger;
        }
        //Create new user
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserInfo NewUser)
        {
            var user = new UserInfo()
            {
                userId= ObjectId.GenerateNewId().ToString(),
                userName = NewUser.userName,
                name = NewUser.name,
                email = NewUser.email,
                dateOfBirth = NewUser.dateOfBirth,
                password = PasswordHash.HashPassword(NewUser.password)
            };
            await _userServices.CreateAsync(user);
            _logger.LogInformation("User Created {0}", user.userName);
            return Ok(user);
            
        }
        
        [HttpGet("{userName}")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> GetUser(string userName)
        {
            var user = await _userServices.FindByuserNameAsync(userName);
            if (user == null)
            {
                return null!;
            }
            
            var userResponse = new UserResponseDto
            {
                email = user.email,
                name = user.name,
                userName = user.userName,
                dateOfBirth = user.dateOfBirth.ToString().Split(' ')[0]
            };
            _logger.LogInformation("Get User {0}", user.userName);
            return Ok(userResponse);
        }

        [HttpPut("{userName}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string userName, [FromBody] UserUpdateDto updateUser)
        {
            var user = await _userServices.FindByuserNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }

            
            user.name = updateUser.name;
            user.userName = updateUser.userName;
            user.dateOfBirth = updateUser.dateOfBirth;
            await _searchServiceMongo.createNewSearch(new UserSearch
            {
                key = "Dopamine_User_Search:" + user.userName + "+" + user.name,
                value = user.userId,
            });
            await _userServices.UpdateAsync(user);
            _logger.LogInformation("User Updated {0}", user.userName);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
        {
            var users = await _userServices.GetAllUsersAsync();
            var result = users.Select(x => new AdminResponseDto
            {
                userName = x.userName,
                name = x.name,
                dateOfBirth = x.dateOfBirth.ToString(),
                email = x.email,
                blockStat = x.isBlocked,
                adminStat = x.role


            }).ToList();
            _logger.LogInformation("Admin requested for Users");
            return Ok(result);

        }
        //block user
        [HttpPut("{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockUser(string userName)
        {
            var user = await _userServices.FindByuserNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }
            user.isBlocked = true;
            await _userServices.UpdateAsync(user);
            _logger.LogWarning("User Blocked {0}", user.userName);
            return Ok();
        }
        //unblock user
        [HttpPut("{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnblockUser(string userName)
        {
            var user = await _userServices.FindByuserNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }
            user.isBlocked = false;
            _logger.LogWarning("User Unblocked By admin userName: {0}", user.userName);
            await _userServices.UpdateAsync(user);
            return Ok();
        }
        //isblock
        [HttpGet("{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> isBlockedByAdmin(string userName)
        {
            var user = await _userServices.FindByuserNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            if (user.isBlocked == true)
            {
                return true;
            }
            return false;
        }
        //make admin
        [HttpPut("{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> makeAdmin(string userName)
        {
            var user = await _userServices.FindByuserNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }

            user.role = "Admin";
            _logger.LogInformation("Admin Made  {0} as Admin", user.userName);
            await _userServices.UpdateAsync(user);
            return Ok();

        }
        //remove admin
        [HttpPut("{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> removeAdmin(string userName)
        {
            var user = await _userServices.FindByuserNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }

            user.role = "User";
            _logger.LogWarning("Admin Removed  {0} from Admin", user.userName);
            await _userServices.UpdateAsync(user);
            return Ok();
        }

        [HttpGet("{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> isAdmin(string userName)
        {
            var user = await _userServices.FindByuserNameAsync(userName);
            if (user == null)
            {
                return false;
            }

            if (user.role == "Admin")
            {
                return true;
            }

            return false;
        }
        [HttpPut("{userName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserbyAdmin(string userName, [FromBody] UserUpdateDto updateUser)
        {
            var user = await _userServices.FindByuserNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }
            user.name = updateUser.name;
            user.userName = updateUser.userName;
            user.dateOfBirth = updateUser.dateOfBirth;
            await _searchServiceMongo.createNewSearch(new UserSearch
            {
                key = "Dopamine_User_Search:" + user.userName + "+" + user.name,
                value = user.userId,
            });
            _logger.LogInformation("Admin Updated User {0}", user.userName);
            await _userServices.UpdateAsync(user);
            return Ok();
        }
    }
}
