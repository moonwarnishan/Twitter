namespace RegisterUser.Models
{
    public class RefreshToken
    {
        public string? Token { get; set; } = null;
        public DateTime ExpireDate { get; set; } = DateTime.Now.AddDays(7);
    }
}
