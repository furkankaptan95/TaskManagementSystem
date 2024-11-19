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

            }).ToList();

        return dtos;
    }

    public async Task<SingleUserDto> GetUserByIdAsync(string id)
    {
        var userEntity = await _mongoDbService.GetUserByIdAsync(id);

        SingleUserDto dto = null;

        if (userEntity is not null)
        {
            dto = new SingleUserDto
            {
                Email = userEntity.Email,
                Username = userEntity.Username,
                Id = id,
            };
        }

        return dto;
    }

    public async Task CreateUserAsync(RegisterDto dto)
    {
        var userEntity = new UserEntity();

        byte[] passwordHash, passwordSalt;

        HashingHelper.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);

        userEntity.Email = dto.Email;
        userEntity.Username = dto.Username;
        userEntity.PasswordHash = passwordHash;
        userEntity.PasswordSalt = passwordSalt;

        await _mongoDbService.CreateUserAsync(userEntity);
    }

    public async Task<ServiceResult> UpdateUserAsync(UpdateUserDto dto)
    {
        var existingUser = await _mongoDbService.GetUserByIdAsync(dto.Id);

        if (existingUser is null)
        {
            return new ServiceResult(false,"User not found.");
        }

        existingUser.Email = dto.Email;
        existingUser.Username = dto.Username;
        existingUser.UpdatedAt = DateTime.UtcNow;

        await _mongoDbService.UpdateUserAsync(dto.Id, existingUser);

        return new ServiceResult(true, "User updated successfully.");
    }

    public async Task DeleteUserAsync(string id)  // id'yi ObjectId olarak alıyoruz
    {
        var existingUser = await _mongoDbService.GetUserByIdAsync(id);  // Mevcut kullanıcıyı al
        if (existingUser == null)
        {
            throw new KeyNotFoundException("User not found");  // Kullanıcı bulunamazsa özel hata
        }

        await _mongoDbService.DeleteUserAsync(id);  // MongoDbService üzerinden kullanıcıyı sil
    }
}
