using RabbitMQ.Client;

namespace AuthAPI.Helpers;

public class RabbitMQConnectionHelper
{
    private readonly string _hostname = "localhost";
    private IConnection _connection;
    private readonly string _queueName = "general_queue";

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
