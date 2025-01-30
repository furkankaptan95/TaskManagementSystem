using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace UserAPI.Helpers;
public class RabbitMQProducer
{
    private readonly RabbitMQConnectionHelper _rabbitMQConnectionHelper;

    public RabbitMQProducer(RabbitMQConnectionHelper rabbitMQConnectionHelper)
    {
        _rabbitMQConnectionHelper = rabbitMQConnectionHelper;
    }

    // İşlem tipi ve mesaj alır
    public void SendMessage<T>(T message, string operationType)
    {
        // Bağlantı ve kanal alma
        var channel = _rabbitMQConnectionHelper.GetChannel();

        // Kuyruk adı belirleme (İşlem türüne göre farklı kuyruklar)
        string queueName = GetQueueNameByOperationType(operationType);

        // Mesajı JSON formatına çevir
        var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        // Kuyruğa mesaj gönder
        channel.BasicPublish(
            exchange: "",              // Default exchange
            routingKey: queueName,     // Kuyruk adı
            basicProperties: null,     // Ekstra özellikler
            body: messageBody         // Mesaj gövdesi
        );

        Console.WriteLine($"Mesaj kuyruğa gönderildi: {JsonSerializer.Serialize(message)}");
    }

    // İşlem tipine göre kuyruk adı belirleme
    private string GetQueueNameByOperationType(string operationType)
    {
        switch (operationType)
        {
            case "CreateUser":
                return "user_create_queue";
            case "UpdateUser":
                return "user_update_queue";
            case "DeleteUser":
                return "user_delete_queue";
            default:
                throw new ArgumentException("Geçersiz işlem tipi.");
        }
    }
}
