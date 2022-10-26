

namespace RegisterUser.Dtos
{
    public class UserUpdateDto
    {
        [Required]
        public string name { get; set; }
        [Required, UserNameExistanceCheck]
        public string userName { get; set; }
        [Required, MinimumAgeValidator(12, ErrorMessage = "Age Must be 12+")]
        public DateTime dateOfBirth { get; set; }
    }
}
