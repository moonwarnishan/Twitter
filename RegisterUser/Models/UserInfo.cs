using Microsoft.AspNetCore.Identity;

namespace RegisterUser.Models
{
    public class UserInfo 
    {
        [BsonElement,BsonId]
        public string? userId { get; set; }
        [Required, MinLength(2), BsonElement]
        public string? name { get; set; }
        [Required, UserNameUnique, BsonElement]
        public string? userName { get; set; }
        [Required, EmailAddress, EmailUnique, BsonElement]
        public string? email { get; set; }
        [Required, MinimumAgeValidator(12, ErrorMessage = "Age Must be 12+")]
        public DateTime? dateOfBirth { get; set; }
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$"), MinLength(8), Required, MaxLength(100), BsonElement]
        public string password { get; set; }
        [Required, BsonElement]
        public string role { get; set; } = "user";
        public DateTime registrationDateTime { get; set; } = DateTime.Now;
        [Required, BsonElement]
        public bool isBlocked { get; set; } = false;

        [Required, BsonElement]

        public RefreshToken? refreshToken { get; set; } = new RefreshToken() { Token = null, ExpireDate = DateTime.Now.AddDays(7), };
    }
}
