namespace AuthAPI.Services;
public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Data { get; set; } // Generic türde veri tutar

    public ServiceResult(bool isSuccess, string message = default, T data = default)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }
}

public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public ServiceResult(bool isSuccess, string message = default)
    {
        IsSuccess = isSuccess;
        Message = message;
    }
}