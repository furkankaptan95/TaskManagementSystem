using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace UserAPI.Helpers;

public class RabbitMQProducer
{
    private readonly RabbitMQConnectionHelper _connectionHelper;
    private IModel _channel;
    public RabbitMQProducer(RabbitMQConnectionHelper connectionHelper)
    {
        _connectionHelper = connectionHelper;
        _channel = _connectionHelper.GetChannel();
    }

    public void SendMessage<T>(T message, string queueName)
    {
        // Mesaj özellikleri
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true; // Mesajın disk üzerine yazılmasını sağlar

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        _channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: properties,
            body: body
        );

    }
}
