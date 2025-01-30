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
        var channel = _rabbitMQConnectionHelper.GetChannel();

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var updateUserDto = JsonConvert.DeserializeObject<UpdateUserDto>(message);

            // Veritabanında kullanıcıyı güncelle
            // authRepository.UpdateUser(updateUserDto);

            Console.WriteLine($"User updated in AuthAPI: {updateUserDto.Id}");
        };

        channel.BasicConsume(
            queue: "user_update_queue",
            autoAck: true,
            consumer: consumer
        );
    }
}
