namespace RegisterUser.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtServices _jwtServices;
        private readonly IRabbitMQConsume _rabbitMqConsume;
        public LoginController(JwtServices jwtServices, IRabbitMQConsume rabbitMqConsume)
        {
            _jwtServices = jwtServices;
            _rabbitMqConsume = rabbitMqConsume;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel M)
        {
            var token =await _jwtServices.Authenticate(M);
            if (token == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            else
            {
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
            }
            catch (Exception)
            {

            }
            
            return Ok();
        }

    }
}
