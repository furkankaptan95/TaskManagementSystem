﻿using UserAPI.DTOs;
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

    public async Task<ServiceResult> ChangePasswordAsync(NewPasswordDto dto)
    {
        var existingUser = await _mongoDbService.GetUserByIdAsync(dto.UserId);

        if (existingUser is null)
        {
            return new ServiceResult(false, "User not found.");
        }

        byte[] passwordHash, passwordSalt;

        HashingHelper.CreatePasswordHash(dto.Password, out passwordHash, out passwordSalt);

        existingUser.PasswordHash = passwordHash;
        existingUser.PasswordSalt = passwordSalt;

        await _mongoDbService.UpdateUserAsync(dto.UserId, existingUser);

        return new ServiceResult(true, "Password changed successfully.");
    }

    public async Task<ServiceResult> DeleteUserAsync(string id)  // id'yi ObjectId olarak alıyoruz
    {
        var existingUser = await _mongoDbService.GetUserByIdAsync(id);

        if (existingUser == null)
        {
            return new ServiceResult(false, "User to delete doesn't exist.");
        }

        await _mongoDbService.DeleteUserAsync(id);

        return new ServiceResult(true, "User deleted successfully.");
    }
}
