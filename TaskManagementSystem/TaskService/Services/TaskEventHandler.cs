using MongoDB.Bson;
using MongoDB.Driver;
using TaskAPI.DTOs;
using TaskAPI.Entities;

namespace TaskAPI.Services;
public class TaskEventHandler : ITaskEventHandler
{
    private readonly MongoDbService _mongoDbService;
    public TaskEventHandler(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    public async Task<ServiceResult> HandleCreateUserAsync(RabbitMQUserCreatedDto createUserDto)
    {
        var userEntity = new UserEntity
        {
            Email = createUserDto.Email,
            Username = createUserDto.Username,
            Id = new ObjectId(createUserDto.Id)
        };

        await _mongoDbService.CreateUserAsync(userEntity);

        return new ServiceResult(true, "User added successfully.");
    }
    public async Task<ServiceResult> HandleDeleteUserAsync(string userId)
    {
        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id, new ObjectId(userId));

        var userEntity = await _mongoDbService.GetUserAsync(userFilter);

        if (userEntity is null)
        {
            return new ServiceResult(false, "User to delete doesn't exist.");
        }

        await _mongoDbService.DeleteUserAsync(userFilter);

        return new ServiceResult(true, "User deleted successfully.");
    }
}
