using RabbitMQ.Client;

namespace UserAPI.Helpers;

public class RabbitMQConnectionHelper
{
    private readonly string _hostname = "localhost";
    private readonly string _queueName = "user_update_queue";
    private IConnection _connection;

    public RabbitMQConnectionHelper()
    {
        var factory = new ConnectionFactory { HostName = _hostname };
        _connection = factory.CreateConnection();
    }

    public IModel GetChannel()
    {
        var channel = _connection.CreateModel();

        // Kuyruğu tanımla
        channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        return channel;
    }
}
