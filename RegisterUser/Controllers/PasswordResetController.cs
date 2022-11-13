

namespace RegisterUser.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly PasswordResetServices _passwordResetServices;
        private readonly ILogger<PasswordResetController> _logger;
        public PasswordResetController(PasswordResetServices passwordResetServices, ILogger<PasswordResetController> logger)
        {
            _passwordResetServices = passwordResetServices;
            _logger = logger;
        }


        [HttpGet("{email}")]
        
        public async Task<ActionResult> RequestPasswordReset(string email)
        {
            var result = _passwordResetServices.RequestPasswordReset(email.ToString());
            if (result == null)
            {
                _logger.LogWarning("User Not Found {0}", email);
                return NotFound(JsonConvert.SerializeObject("User Not Found"));
            }
            _logger.LogInformation("Password reset request for {0}", email);
            return Ok(JsonConvert.SerializeObject(result));
        }
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                await _passwordResetServices.ResetPassword(resetPasswordDto);
                _logger.LogInformation("Password Changed for {0}", resetPasswordDto.userId);
                return Ok(JsonConvert.SerializeObject("Password Changed"));
            }
            catch (Exception e)
            {
                _logger.LogError("Password Change Failed for {0}", resetPasswordDto.userId);
                throw new NotImplementedException();
            }
        }



    }
}