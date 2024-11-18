namespace TaskAPI.Services;
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public ServiceResult(bool isSuccess,string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }
}
