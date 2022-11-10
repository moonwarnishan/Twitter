using IConnection = RabbitMQ.Client.IConnection;

namespace RegisterUser.Services
{
    public class RabbitMQNotification : IRabbitMQNotification
    {
        private static readonly ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;
        protected readonly IServiceProvider _serviceProvider;
        private readonly NotificationHub _hub;
        public RabbitMQNotification(IServiceProvider serviceProvider,NotificationHub hub)
        {
            _factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://uslpaenl:EhK787ZeOdfT8Cerm4svZN2p53pD0mtl@beaver.rmq.cloudamqp.com/uslpaenl")
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceProvider = serviceProvider;
            _hub = hub;
        }

        public async Task Connect()
        {

            var followers = new List<string>();


            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (m, e) =>
            {
                byte[] body = e.Body.ToArray();
                var msg = (NotificationDto)JsonConvert.DeserializeObject<NotificationDto>(Encoding.UTF8.GetString(body))!; ;
                _hub.SendChatMessage(msg.receiverUserName,msg);
            };

            _channel.BasicConsume(queue: "DopamineNotification",
                autoAck: true,
                consumer: consumer);
        }
    }
}
