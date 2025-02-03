using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AuthAPI.DTOs;
using AuthAPI.Services;

namespace AuthAPI.Helpers;

public class RabbitMQConsumer : BackgroundService
{
    private readonly RabbitMQConnectionHelper _rabbitMQConnectionHelper;
    private readonly IAuthEventHandler _authEventHandler;
    private IModel _channel;

    public RabbitMQConsumer(RabbitMQConnectionHelper rabbitMQConnectionHelper, IAuthEventHandler authEventHandler)
    {
        _rabbitMQConnectionHelper = rabbitMQConnectionHelper;
        _authEventHandler = authEventHandler;
        _channel = _rabbitMQConnectionHelper.GetChannel();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ListenQueue("user_update_queue", stoppingToken); 
        ListenQueue("user_role_update_queue", stoppingToken); 
        ListenQueue("user_delete_queue_authapi", stoppingToken);

        return Task.CompletedTask;
    }

    private void ListenQueue(string queueName, CancellationToken stoppingToken)
    {
        // Kuyruk her zaman dinleniyor
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            // Consumer durdurulduğunda işlem yapmamalıyız
            if (stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Consumer stopped.");
                return;
            }

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Message received from {queueName}: {message}");

            // Mesajı işliyoruz
            ProcessMessage(queueName, message);
        };

        // Kuyruğu dinlemeye başlıyoruz
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        Console.WriteLine($"Listening on {queueName}...");
    }

    private void ProcessMessage(string queueName, string message)
    {
        switch (queueName)
        {
            case "user_update_queue":
                var updateUserDto = JsonConvert.DeserializeObject<UpdateUserDto>(message);
                _authEventHandler.HandleUserUpdateAsync(updateUserDto);
                break;

            case "user_role_update_queue":
                var updateUserRoleDto = JsonConvert.DeserializeObject<UpdateRoleDto>(message);

                _authEventHandler.HandleUserRoleUpdateAsync(updateUserRoleDto);
                break;

            case "user_delete_queue_authapi":
                var userId = JsonConvert.DeserializeObject<string>(message);
                _authEventHandler.HandleDeleteUserAsync(userId);
                break;

            default:
                Console.WriteLine($"[!] Unknown queue message received: {queueName}");
                break;
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        return base.StopAsync(cancellationToken);
    }
}
