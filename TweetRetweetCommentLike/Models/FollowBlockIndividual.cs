namespace TweetRetweetCommentLike.Models
{
    public class FollowBlockIndividual
    {
        [Required, BsonId]
        public string userName { get; set; }
        [Required]
        public List<string> FollowedByUsersNames { get; set; } = new List<string>();

        [Required] public List<string> blockedByUsersNames { get; set; } = new List<string>();
    }
}
