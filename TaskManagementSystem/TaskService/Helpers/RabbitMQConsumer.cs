using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using TaskAPI.Services;
using Newtonsoft.Json;
using TaskAPI.DTOs;

namespace TaskAPI.Helpers;
public class RabbitMQConsumer : BackgroundService
{
    private readonly RabbitMQConnectionHelper _rabbitMQConnectionHelper;
    private readonly ITaskEventHandler _taskEventHandler;
    private IModel _channel;

    public RabbitMQConsumer(RabbitMQConnectionHelper rabbitMQConnectionHelper, ITaskEventHandler taskEventHandler)
    {
        _rabbitMQConnectionHelper = rabbitMQConnectionHelper;
        _taskEventHandler = taskEventHandler;
        _channel = _rabbitMQConnectionHelper.GetChannel();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ListenQueue("user_create_queue_taskapi", stoppingToken);
        ListenQueue("user_delete_queue_taskapi", stoppingToken);

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
            case "user_create_queue_taskapi":
                var newUserDto = JsonConvert.DeserializeObject<RabbitMQUserCreatedDto>(message);
                _taskEventHandler.HandleCreateUserAsync(newUserDto);
                break;

            case "user_delete_queue_taskapi":
                var userId = JsonConvert.DeserializeObject<string>(message);
                _taskEventHandler.HandleDeleteUserAsync(userId);
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
