namespace TweetRetweetCommentLike.Interfaces
{
    public interface ILikeCommentRetweetServices
    {
        public Task CreateOrDeleteLike(string tweetId, string receiverUserName, string userName);
        public Task CreateOrDeleteReTweet(string tweetId, string receiverUserName, RetweetDto retweet);
        public Task CreateComment(string tweetId, string receiverUserName, CommentDto comment);
        public Task DeleteComment(string tweetId, string commentId, string userName);
        public Task<CommentLikeRetweet> getAll(string tweetId);
        public Task Delete(string tweetId);
    }
}
