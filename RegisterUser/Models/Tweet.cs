namespace RegisterUser.Models
{
    public class Tweet
    {
        [BsonId]
        public string? tweetId { get; set; }
        [Required]
        public string? userName { get; set; }
        [Required,MinLength(1)]
        public string? tweetText { get; set; }

        [Required] 
        public string tweetTime { get; set; } = DateTime.Now.ToString();

    }
}
