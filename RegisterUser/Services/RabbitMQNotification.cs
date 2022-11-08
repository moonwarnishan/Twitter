using System.Xml.Linq;
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
            _factory = new ConnectionFactory() { HostName = "localhost" };
            //_factory.Uri = new Uri("amqps://kkeawubu:x17GNxtgIQWM74zyTnuLoaSZcQUrKNvD@armadillo.rmq.cloudamqp.com/kkeawubu");
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
