namespace TweetRetweetCommentLike.Services
{
    public class RabbitMQDeleteServices : IRabbitMqDeleteService
    {
        private readonly IFollowBlockIndividualServices _followBlockIndividual;
        private IRabbitMQService _rabbitMQServices;
        private IMongoCollection<CommentLikeRetweet> _likeCommentRetweet;
        private IMongoCollection<TweetSearchDto> _tweetSearch;
        private readonly ILikeCommentRetweetServices _likeCommentRetweetService;
        public RabbitMQDeleteServices(IFollowBlockIndividualServices followBlockIndividual,
            IOptions<DatabaseSetting.DatabaseSetting> db,
            ILikeCommentRetweetServices likeCommentRetweetService, IRabbitMQService mqService)
        {
            _rabbitMQServices = mqService;
            var client = new MongoClient(db.Value.connectionString);
            var database = client.GetDatabase(db.Value.databaseName);
            _followBlockIndividual = followBlockIndividual;
            _likeCommentRetweetService = likeCommentRetweetService;
            _tweetSearch = database.GetCollection<TweetSearchDto>(db.Value.hashSearchCollection);
        }

        public async Task Send(string userName, string tweetId)
        {
            
            var followers = new List<string>();
            if (await _followBlockIndividual.GetAllFollowers(userName) == null)
            {
                followers.Add(userName);
            }
            else
            {
                followers = await _followBlockIndividual.GetAllFollowers(userName);
                followers.Add(userName);
            }
            await _likeCommentRetweetService.Delete(tweetId);
            await _tweetSearch.FindOneAndDeleteAsync(x => x.tweetId == tweetId);
            using (IModel channel = _rabbitMQServices.CreateChannel().CreateModel())
            {
                channel.ExchangeDeclare("Dopaminedelete:" + userName, ExchangeType.Fanout);
                foreach (var follower in followers)
                {
                    channel.QueueDeclare("Dopaminedelete:" + follower, true, false, false);
                    channel.QueueBind("Dopaminedelete:" + follower, "Dopaminedelete:" + userName, follower);
                }

                byte[] body = Encoding.UTF8.GetBytes(tweetId);
                channel.BasicPublish(exchange: "Dopaminedelete:" + userName,
                    routingKey: "",
                    basicProperties: null,
                    body: body);
                foreach (var follower in followers)
                {
                    channel.QueueUnbind(follower, "Dopaminedelete:" + userName, follower);
                }

                channel.ExchangeDelete("Dopaminedelete:" + userName);
            }
        }
    }
}
