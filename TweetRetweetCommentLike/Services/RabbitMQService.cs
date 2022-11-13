
namespace TweetRetweetCommentLike.Services
{
    public class RabbitMQService:IRabbitMQService
    {
        public IConnection CreateChannel()
        {
            ConnectionFactory connection = new ConnectionFactory()
            {
                Uri = new Uri("amqps://klwxffmu:lXXsHy8GAIyDzfsX1ZWPo-Sso1lU4Z62@beaver.rmq.cloudamqp.com/klwxffmu")
            };
            connection.DispatchConsumersAsync = true;
            var channel = connection.CreateConnection();
            return channel;
        }
    }
}
