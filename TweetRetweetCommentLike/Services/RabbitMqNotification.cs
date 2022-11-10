namespace TweetRetweetCommentLike.Services
{
    public class RabbitMqNotification : IRabbitMqNotification
    {

        public RabbitMqNotification() 
        {

        }

        public async Task Send(NotificationDto Notification)
        {
            ConnectionFactory _factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://uslpaenl:EhK787ZeOdfT8Cerm4svZN2p53pD0mtl@beaver.rmq.cloudamqp.com/uslpaenl")
            };
           
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
