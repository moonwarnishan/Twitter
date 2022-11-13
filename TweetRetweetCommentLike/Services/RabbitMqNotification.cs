namespace TweetRetweetCommentLike.Services
{
    public class RabbitMqNotification : IRabbitMqNotification
    {
        private readonly IRabbitMQService _rabbitMQServices;
        public RabbitMqNotification(IRabbitMQService mqService)
        {
            _rabbitMQServices = mqService;
        }
        
        public async Task Send(NotificationDto Notification)
        {
            
            var followers = new List<string>();
           
            using (IModel channel =_rabbitMQServices.CreateChannel().CreateModel())
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
