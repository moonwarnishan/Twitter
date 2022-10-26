

namespace RegisterUser.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {
        private readonly PasswordResetServices _passwordResetServices;
        public PasswordResetController(PasswordResetServices passwordResetServices)
        {
            _passwordResetServices = passwordResetServices;
        }


        [HttpGet("{email}")]
        
        public async Task<ActionResult> RequestPasswordReset(string email)
        {
            var result = _passwordResetServices.RequestPasswordReset(email.ToString());
            if (result == null)
            {
                return NotFound(JsonConvert.SerializeObject("User Not Found"));
            }
            return Ok(JsonConvert.SerializeObject(result));
        }
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                await _passwordResetServices.ResetPassword(resetPasswordDto);
                return Ok(JsonConvert.SerializeObject("Password Changed"));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



    }
}