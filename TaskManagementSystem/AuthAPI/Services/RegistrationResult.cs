using AuthAPI.Enums;

namespace AuthAPI.Services;
public class RegistrationResult
{
    public bool IsSuccess { get; set; }
    public RegistrationError Error { get; set; }
    public string? Message { get; set; }
    public RegistrationResult(bool isSuccess, string? message, RegistrationError error)
    {
        IsSuccess = isSuccess;
        Error = error;
        Message = message;
    }
}
