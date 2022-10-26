using TweetRetweetCommentLike.Interfaces;

namespace TweetRetweetCommentLike.Services
{
    public class TweetServices : ITweetServices
    {
        public readonly IRabbitMqPublish _rabbitMqPublish;

        public TweetServices(IRabbitMqPublish rabbitMqPublish)
        {
            _rabbitMqPublish = rabbitMqPublish;

        }

        public async Task createTweet(Tweet tweet)
        {
            await _rabbitMqPublish.Send(tweet);
        }

        public async Task DeleteTweet(string userName,string tweetId)
        {
            
        }
    }
}
