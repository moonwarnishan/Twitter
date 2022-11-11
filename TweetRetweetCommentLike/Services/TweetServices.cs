namespace TweetRetweetCommentLike.Services
{
    public class TweetServices : ITweetServices
    {
        public readonly IRabbitMqPublish _rabbitMqPublish;
        public readonly IRabbitMqDeleteService _DeleteServices;

        public TweetServices(IRabbitMqPublish rabbitMqPublish, IRabbitMqDeleteService deleteServices, IOptions<DatabaseSetting.DatabaseSetting> dbsettings)
        {

            _rabbitMqPublish = rabbitMqPublish;
            _DeleteServices = deleteServices;
        }

        public async Task createTweet(Tweet tweet)
        {
            await _rabbitMqPublish.Send(tweet);
        }

        public async Task DeleteTweet(string userName,string tweetId)
        {
            await _DeleteServices.Send(userName, tweetId);
        }
    }
}
