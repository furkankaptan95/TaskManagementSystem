using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace UserAPI.Helpers;

public class RabbitMQProducer
{
    private readonly RabbitMQConnectionHelper _connectionHelper;

    public RabbitMQProducer(RabbitMQConnectionHelper connectionHelper)
    {
        _connectionHelper = connectionHelper;
    }

    public void SendMessage<T>(T message, string messageType)
    {
        var queueName = "general_queue";

        using var channel = _connectionHelper.GetChannel(); // Doğru kullanım

        // Mesaj özellikleri
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true; // Mesajın disk üzerine yazılmasını sağlar

        var messageObject = new RabbitMQMessage
        {
            OperationType = messageType, // İşlem türünü mesaj içine koy
            Data = message
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messageObject));

        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: properties,
            body: body
        );

    }
}
