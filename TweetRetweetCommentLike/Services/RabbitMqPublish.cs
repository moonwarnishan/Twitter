﻿namespace TweetRetweetCommentLike.Services
{
    public class RabbitMqPublish : IRabbitMqPublish
    {
        private readonly IFollowBlockIndividualServices _followBlockIndividual;
        private IMongoCollection<CommentLikeRetweet> _likeCommentRetweet;
        private readonly IMongoCollection<TweetSearchDto> _tweetSearch;
        public RabbitMqPublish(IFollowBlockIndividualServices followBlockIndividual,IOptions<DatabaseSetting.DatabaseSetting> db)
        {
            var client = new MongoClient(db.Value.connectionString);
            var database = client.GetDatabase(db.Value.databaseName);
            _tweetSearch = database.GetCollection<TweetSearchDto>(db.Value.hashSearchCollection);
            _likeCommentRetweet = database.GetCollection<CommentLikeRetweet>(db.Value.likeCommentRetweetCollectionName);
            _followBlockIndividual = followBlockIndividual;
        }
        public async Task Send(Tweet tweet)
        {

            ConnectionFactory _factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://uslpaenl:EhK787ZeOdfT8Cerm4svZN2p53pD0mtl@beaver.rmq.cloudamqp.com/uslpaenl")
            };

            var followers = new List<string>();
            if (await _followBlockIndividual.GetAllFollowers(tweet.userName) == null)
            {
                followers.Add(tweet.userName);
            }
            else
            {
                followers = await _followBlockIndividual.GetAllFollowers(tweet.userName);
                followers.Add(tweet.userName);
            }
            tweet.tweetId = Guid.NewGuid().ToString();

            var regex = new Regex(@"#\w+");
            var dto = new TweetSearchDto();
            
            var matches = regex.Matches(tweet.tweetText);
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    dto.key += match.ToString();
                }

                dto.tweetId = tweet.tweetId;
                dto.tweetText = tweet.tweetText;
                dto.userName = tweet.userName;

                await _tweetSearch.InsertOneAsync(dto);
            }
            
            var likeCommentRetweet = new CommentLikeRetweet()
            {
                tweetId = tweet.tweetId,
                comments = new List<CommentDto>(),
                likes = new List<string>(),
                retweets = new List<RetweetDto>()
            };
            await _likeCommentRetweet.InsertOneAsync(likeCommentRetweet);

            Dictionary<String, Object> args = new Dictionary<String, Object>();
            args.Add("x-message-ttl", 86400000);
            using (IConnection connection = _factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("Dopamine:" + tweet.userName, ExchangeType.Fanout);
                foreach (var follower in followers)
                {
                    channel.QueueDeclare("Dopamine:"+follower, true, false, false,args);
                    channel.QueueBind("Dopamine:"+follower, "Dopamine:" + tweet.userName, follower);
                }
                byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tweet));
                 channel.BasicPublish(exchange: "Dopamine:"+ tweet.userName,
                    routingKey: "",
                    basicProperties: null,
                    body: body);
                foreach (var follower in followers)
                {
                    channel.QueueUnbind(follower, "Dopamine:" + tweet.userName, follower);
                }
                channel.ExchangeDelete("Dopamine:" + tweet.userName);
            }
        }
    }
}
