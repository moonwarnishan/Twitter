namespace RegisterUser.Models
{
    public class ReturnToken
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? userName { get; set; }
        public string? role { get; set; }

    }
}
