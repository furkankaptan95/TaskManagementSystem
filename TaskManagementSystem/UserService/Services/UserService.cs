using MongoDB.Bson;
using MongoDB.Driver;
using UserAPI.DTOs;
using UserAPI.Entities;

namespace UserAPI.Services;
public class UserService
{
    private readonly MongoDbService _mongoDbService;
    public UserService(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    public async Task<List<AllUsersDto>> GetAllUsersAsync()
    {
        var entities = await _mongoDbService.GetAllUsersAsync();

        var dtos = entities
            .Select(user => new AllUsersDto
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                Username = user.Username,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
            }).ToList();

        return dtos;
    }
    public async Task<ServiceResult<SingleUserDto>> GetUserByIdAsync(string id)
    {
        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id, new ObjectId(id));
        var userEntity = await _mongoDbService.GetUserAsync(userFilter);

        if(userEntity is null)
        {
            return new ServiceResult<SingleUserDto>(false, "User not found.");
        }

        var dto = new SingleUserDto
        {
            Id = userEntity.Id.ToString(),
            Email = userEntity.Email,
            Username= userEntity.Username,
            Firstname = userEntity.Firstname,
            Lastname = userEntity.Lastname,
            Role = userEntity.Role,
            CreatedAt = userEntity.CreatedAt,
        };

        return new ServiceResult<SingleUserDto>(true, null, dto);

    }
    public async Task<ServiceResult> UpdateUserAsync(UpdateUserDto dto)
    {
        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id, new ObjectId(dto.Id));
        var userEntity = await _mongoDbService.GetUserAsync(userFilter);

        if (userEntity is null)
        {
            return new ServiceResult(false,"User not found.");
        }

        var update = Builders<UserEntity>.Update
            .Set(user => user.Firstname, dto.Firstname)
            .Set(user => user.Lastname, dto.Lastname)
            .Set(user => user.UpdatedAt, DateTime.UtcNow);

        await _mongoDbService.UpdateUserAsync(userFilter, update);

        return new ServiceResult(true, "User updated successfully.");
    }
    public async Task<ServiceResult> DeleteUserAsync(string id)
    {
        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id, new ObjectId(id));
        var userEntity = await _mongoDbService.GetUserAsync(userFilter);

        if (userEntity is null)
        {
            return new ServiceResult(false, "User to delete doesn't exist.");
        }

        await _mongoDbService.DeleteUserAsync(userFilter);

        return new ServiceResult(true, "User deleted successfully.");
    }
    public async Task<ServiceResult> UpdateUserRoleAsync(UpdateRoleDto dto)
    {
        var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id, new ObjectId(dto.UserId));
        var userEntity = await _mongoDbService.GetUserAsync(userFilter);

        if (userEntity is null)
        {
            return new ServiceResult(false, "User not found.");
        }

        var update = Builders<UserEntity>.Update
            .Set(user => user.Role, dto.Role)
            .Set(user => user.UpdatedAt, DateTime.UtcNow);

        await _mongoDbService.UpdateUserAsync(userFilter, update);

        return new ServiceResult(true, "User Role updated successfully.");
    }
}
