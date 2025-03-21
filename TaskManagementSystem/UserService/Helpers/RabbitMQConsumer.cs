﻿using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using UserAPI.Services;
using UserAPI.DTOs;

namespace UserAPI.Helpers;
public class RabbitMQConsumer : BackgroundService
{
    private readonly RabbitMQConnectionHelper _rabbitMQConnectionHelper;
    private readonly IUserEventHandler _userEventHandler;
    private IModel _channel;

    public RabbitMQConsumer(RabbitMQConnectionHelper rabbitMQConnectionHelper, IUserEventHandler userEventHandler)
    {
        _rabbitMQConnectionHelper = rabbitMQConnectionHelper;
        _userEventHandler = userEventHandler;
        _channel = _rabbitMQConnectionHelper.GetChannel();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ListenQueue("user_create_queue_userapi", stoppingToken);

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
            case "user_create_queue_userapi":
                var newUserDto = JsonConvert.DeserializeObject<RabbitMQUserCreatedDto>(message);
                _userEventHandler.HandleCreateUserAsync(newUserDto);
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
