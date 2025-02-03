using RabbitMQ.Client;

namespace AuthAPI.Helpers;

public class RabbitMQConnectionHelper
{
    private readonly string _hostname = "localhost";
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQConnectionHelper()
    {
        CreateConnection();
    }

    private void CreateConnection()
    {
        // Bağlantı varsa ve açık ise devam et
        if (_connection != null && _connection.IsOpen)
            return;

        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            AutomaticRecoveryEnabled = true,  // Otomatik kurtarma aktif
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)  // Yeniden bağlanma için süre
        };

        // Bağlantı kur
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Kuyruğu sadece bir kez tanımla
        _channel.QueueDeclare(queue: "user_update_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "user_role_update_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "user_delete_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "user_create_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "general_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    public IModel GetChannel() => _channel;
    public IConnection GetConnection() => _connection;
}
