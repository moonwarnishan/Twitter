namespace RegisterUser.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtServices _jwtServices;
        private readonly IRabbitMQConsume _rabbitMqConsume;
        private readonly IRabbitMqDeleteService _deleteService;
        private readonly UserServices _userServices;
        private readonly ILogger<LoginController> _logger;
        private readonly IRedisServices _redisServices;
        public LoginController(JwtServices jwtServices,
            IRabbitMQConsume rabbitMqConsume,
            IRabbitMqDeleteService deleteService,
            UserServices userServices,
            ILogger<LoginController> logger,
            IRedisServices redisServices)
        {
            _jwtServices = jwtServices;
            _rabbitMqConsume = rabbitMqConsume;
            _deleteService = deleteService;
            _userServices = userServices;
            _logger = logger;
            _redisServices = redisServices;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel M)
        {
            var token =await _jwtServices.Authenticate(M);
            if (token == null)
            {
                _logger.LogWarning("Username or password is incorrect");
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            else
            {
                
                _logger.LogInformation("{0} logged in.",M.userName);

                await _rabbitMqConsume.Connect(M.userName);
                await _redisServices.SetCacheValueAsync(M.userName);
                return Ok(token);

            }
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> ConsumeTweet(string userName)
        {
            await _rabbitMqConsume.Connect(userName);
            _logger.LogInformation("Consume tweet from {0}", userName);
            return Ok();
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> DeleteTweet(string userName)
        {
            await _deleteService.Connect(userName);
            _logger.LogInformation("Delete tweet from {0} timeline", userName);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out.");
            return Ok(HttpContext.User);
        }



    }
}
