using TaskAPI.DTOs;

namespace TaskAPI.Services;
public interface ITaskEventHandler
{
    Task<ServiceResult> HandleCreateUserAsync(RabbitMQUserCreatedDto createUserDto);
    Task<ServiceResult> HandleDeleteUserAsync(string userId);
}
