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

    public async Task<UserEntity> GetUserByIdAsync(string id)  // ObjectId olarak alıyoruz
    {
        var user = await _mongoDbService.GetUserByIdAsync(id);  // MongoDbService üzerinden kullanıcı bilgilerini al

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");  // Kullanıcı bulunamazsa özel hata
        }

        return user;
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

    public async Task UpdateUserAsync(string id, UserEntity updatedUser)  // id'yi ObjectId olarak alıyoruz
    {
        var existingUser = await _mongoDbService.GetUserByIdAsync(id);  // Mevcut kullanıcıyı al
        if (existingUser == null)
        {
            throw new KeyNotFoundException("User not found");  // Kullanıcı bulunamazsa özel hata
        }

        updatedUser.Id = existingUser.Id;  // Güncellenmiş kullanıcıda eski ID'yi koru
        await _mongoDbService.UpdateUserAsync(id, updatedUser);  // MongoDbService üzerinden kullanıcıyı güncelle
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
