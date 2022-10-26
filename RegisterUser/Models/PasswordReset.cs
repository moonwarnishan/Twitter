namespace RegisterUser.Models
{
    public class PasswordReset
    {
        [BsonId]
        public string userId { get; set; }
        public string token { get; set; }
        public string expiryDate { get; set; }=DateTime.Now.AddHours(1).ToString();
    }
}
