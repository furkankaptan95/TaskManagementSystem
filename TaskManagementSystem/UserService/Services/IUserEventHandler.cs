using UserAPI.DTOs;

namespace UserAPI.Services;
public interface IUserEventHandler
{
    Task<ServiceResult> HandleCreateUserAsync(RabbitMQUserCreatedDto createUserDto);
}
