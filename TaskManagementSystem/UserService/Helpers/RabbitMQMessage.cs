namespace UserAPI.Helpers;
public class RabbitMQMessage
{
    public string OperationType { get; set; }
    public object Data { get; set; }
}
