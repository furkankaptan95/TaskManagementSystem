using MongoDB.Bson;
using UserAPI.DTOs;
using UserAPI.Entities;

namespace UserAPI.Services;
public class UserEventHandler : IUserEventHandler
{
    private readonly MongoDbService _mongoDbService;
    public UserEventHandler(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    public async Task<ServiceResult> HandleCreateUserAsync(RabbitMQUserCreatedDto createUserDto)
    {
        var userEntity = new UserEntity
        {
            Email = createUserDto.Email,
            Firstname = createUserDto.Firstname,
            Lastname = createUserDto.Lastname,
            Username = createUserDto.Username,
            Id = new ObjectId(createUserDto.Id)
        };

        await _mongoDbService.CreateUserAsync(userEntity);

        return new ServiceResult(true, "User added successfully.");
    }
}
