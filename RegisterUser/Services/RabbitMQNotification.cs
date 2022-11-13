using IConnection = RabbitMQ.Client.IConnection;

namespace RegisterUser.Services
{
    public class RabbitMQNotification : IRabbitMQNotification,IDisposable
    {
        private static readonly ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;
        protected readonly IServiceProvider _serviceProvider;
        private readonly NotificationHub _hub;
        public RabbitMQNotification(IServiceProvider serviceProvider, NotificationHub hub)
        {
            _factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://klwxffmu:lXXsHy8GAIyDzfsX1ZWPo-Sso1lU4Z62@beaver.rmq.cloudamqp.com/klwxffmu")
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
                _hub.SendChatMessage(msg.receiverUserName, msg);
            };

            _channel.BasicConsume(queue: "DopamineNotification",
                autoAck: true,
                consumer: consumer);
        }

        public void Dispose()
        {
            if (_channel.IsOpen)
                _channel.Close();
            if (_connection.IsOpen)
                _connection.Close();
        }
    }
}