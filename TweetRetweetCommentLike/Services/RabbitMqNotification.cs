namespace TweetRetweetCommentLike.Services
{
    public class RabbitMqNotification : IRabbitMqNotification
    {

        public RabbitMqNotification() 
        {

        }

        public async Task Send(NotificationDto Notification)
        {
            ConnectionFactory _factory = new ConnectionFactory() { HostName = "localhost" };
            //_factory.Uri = new Uri("amqps://kkeawubu:x17GNxtgIQWM74zyTnuLoaSZcQUrKNvD@armadillo.rmq.cloudamqp.com/kkeawubu");
            var followers = new List<string>();

            using (IConnection connection = _factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("DopamineNotification", ExchangeType.Direct);
                channel.QueueDeclare("DopamineNotification", true, false, false);
                channel.QueueBind("DopamineNotification", "DopamineNotification","");

                byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Notification));
                channel.BasicPublish(exchange: "DopamineNotification",
                    routingKey: "",
                    basicProperties: null,
                    body: body);

            }
        }
    }
}
