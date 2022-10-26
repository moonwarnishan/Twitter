using System.ComponentModel.DataAnnotations;

namespace RegisterUser.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is Required")]
        public string userName { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string password { get; set; }
    }
}
