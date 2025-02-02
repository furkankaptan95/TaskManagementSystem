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
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = _rabbitMQConnectionHelper.GetChannel();
        _channel = connection;

        _channel.QueueDeclare(
            queue: "general_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"[*] Message received: {message}");

            try
            {
                var receivedMessage = JsonConvert.DeserializeObject<RabbitMQMessage>(message);
                ProcessMessage(receivedMessage);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error processing message: {ex.Message}");
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: "general_queue", autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private void ProcessMessage(RabbitMQMessage message)
    {
        switch (message.OperationType)
        {
            case "UpdateUser":
                var updateUserDto = JsonConvert.DeserializeObject<UpdateUserDto>(message.Data.ToString());
                _authEventHandler.HandleUserUpdateAsync(updateUserDto);
                break;

            case "UpdateUserRole":
                var updateUserRoleDto = JsonConvert.DeserializeObject<UpdateRoleDto>(message.Data.ToString());

                _authEventHandler.HandleUserRoleUpdateAsync(updateUserRoleDto);
                break;

            case "DeleteUser":
                var userId = message.Data.ToString();
                _authEventHandler.HandleDeleteUserAsync(userId);
                break;

            default:
                Console.WriteLine($"[!] Unknown queue message received: {message.OperationType}");
                break;
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        return base.StopAsync(cancellationToken);
    }
}
