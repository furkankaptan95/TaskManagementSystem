using AuthAPI.DTOs;
using AuthAPI.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthAPI.Services;
public class AuthEventHandler : IAuthEventHandler
{
    private readonly MongoDbService _mongoDbService;
    public AuthEventHandler(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
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

    public async Task<ServiceResult> HandleUserRoleUpdateAsync(UpdateRoleDto dto)
    {
        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id, new ObjectId(dto.UserId));
        var userEntity = await _mongoDbService.GetUserAsync(userFilter);

        if (userEntity is null)
        {
            return new ServiceResult(false, "User not found.");
        }

        var update = Builders<UserEntity>.Update
            .Set(user => user.Role, dto.Role);
            

        await _mongoDbService.UpdateUserAsync(userFilter, update);

        return new ServiceResult(true, "User updated successfully.");
    }

    public async Task<ServiceResult> HandleUserUpdateAsync(UpdateUserDto dto)
    {
        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id, new ObjectId(dto.Id));
        var userEntity = await _mongoDbService.GetUserAsync(userFilter);

        if (userEntity is null)
        {
            return new ServiceResult(false, "User not found.");
        }

        var update = Builders<UserEntity>.Update
            .Set(user => user.Firstname, dto.Firstname)
            .Set(user => user.Lastname, dto.Lastname);
            

        await _mongoDbService.UpdateUserAsync(userFilter, update);

        return new ServiceResult(true, "User updated successfully.");
    }
}
