namespace RegisterUser.Dtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string userId { get; set; }
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$"), MinLength(8), Required, MaxLength(100), BsonElement]
        public string password { get; set; }
        [Required]
        public string token { get; set; }
    }
}
