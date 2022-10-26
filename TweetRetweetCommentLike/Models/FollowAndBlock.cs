namespace TweetRetweetCommentLike.Models
{
    public class FollowAndBlock
    {
        [Required, BsonId]
        public string folloeName { get; set; }
        [Required]
        public List<string> followerNames { get; set; } = new List<string>();

        [Required]
        public List<string> blockedNames { get; set; } = new List<string>();
    }
}
