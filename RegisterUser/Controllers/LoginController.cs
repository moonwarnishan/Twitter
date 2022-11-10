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
        public LoginController(JwtServices jwtServices, IRabbitMQConsume rabbitMqConsume, IRabbitMqDeleteService deleteService, UserServices userServices, ILogger<LoginController> logger)
        {
            _jwtServices = jwtServices;
            _rabbitMqConsume = rabbitMqConsume;
            _deleteService = deleteService;
            _userServices = userServices;
            _logger = logger;
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
                var user =await _userServices.FindByuserNameAsync(M.userName);
                var claim = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.userName),
                    new Claim(ClaimTypes.Email,user.email),
                    new Claim(ClaimTypes.Role,user.role)
                };
                var claimsIdentity = new ClaimsIdentity(
                    claim, CookieAuthenticationDefaults.AuthenticationScheme);
                //set claim when login
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    IsPersistent = true,
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                _logger.LogInformation("{0} logged in.",M.userName);

                try
                {
                    await _rabbitMqConsume.Connect(M.userName);
                }
                catch (Exception)
                {

                }
                return Ok(token);

            }
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> ConsumeTweet(string userName)
        {
            try
            {
                await _rabbitMqConsume.Connect(userName);
                _logger.LogInformation("Consume tweet from {0}", userName);
            }
            catch (Exception)
            {

            }
            
            return Ok();
        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> DeleteTweet(string userName)
        {
            try
            {
                await _deleteService.Connect(userName);
                _logger.LogInformation("Delete tweet from {0} timeline", userName);
            }
            catch (Exception)
            {

            }

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
