using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AuthAPI.DTOs;

namespace AuthAPI.Helpers;

public class RabbitMQConsumer
{
    private readonly RabbitMQConnectionHelper _rabbitMQConnectionHelper;

    public RabbitMQConsumer(RabbitMQConnectionHelper rabbitMQConnectionHelper)
    {
        _rabbitMQConnectionHelper = rabbitMQConnectionHelper;
    }

    public void StartConsuming()
    {
        Task.Run(() =>
        {
            using var channel = _rabbitMQConnectionHelper.GetChannel();

            // Kuyruğun varlığını garanti altına al
            channel.QueueDeclare(
                queue: "general_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Newtonsoft.Json kullanarak deserialization işlemi
                var receivedMessage = JsonConvert.DeserializeObject<RabbitMQMessage>(message);
                
                ProcessMessage(receivedMessage);
            };

            channel.BasicConsume(
                queue: "general_queue",
                autoAck: true,
                consumer: consumer
            );

            while (true) { Task.Delay(1000).Wait(); }
        });
    }

    private void ProcessMessage(RabbitMQMessage message)
    {
        switch (message.OperationType)
        {
            case "UpdateUser":
                var updateUserDto = JsonConvert.DeserializeObject<UpdateUserDto>(message.Data.ToString());
                
                // authRepository.UpdateUser(updateUserDto);
                break;

            default:
                Console.WriteLine($"[!] Unknown queue message received: ");
                break;
        }
    }
}
