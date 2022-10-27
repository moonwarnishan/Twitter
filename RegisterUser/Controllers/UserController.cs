
namespace RegisterUser.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly UserServices _userServices;
        private readonly SearchServiceMongo _searchServiceMongo;

        public UserController(UserServices userServices, SearchServiceMongo searchServiceMongo)
        {
            _userServices = userServices;
            _searchServiceMongo = searchServiceMongo;
        }
        
            //Create new user
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User NewUser)
        {
            var user = new User()
            {
                userId= ObjectId.GenerateNewId().ToString(),
                userName = NewUser.userName,
                name = NewUser.name,
                email = NewUser.email,
                dateOfBirth = NewUser.dateOfBirth,
                password = PasswordHash.HashPassword(NewUser.password)
            };
            await _userServices.CreateAsync(user);
            return Ok();
            
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
            
            await _userServices.UpdateAsync(user);
            return Ok();
        }
    }
}
