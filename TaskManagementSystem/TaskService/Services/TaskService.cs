﻿using MongoDB.Driver;
using MongoDB.Bson;
using TaskAPI.Entities;
using TaskAPI.DTOs;

namespace TaskAPI.Services;

public class TaskService
{
    private readonly MongoDbService _mongoDbService;

    public TaskService(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    // Tüm görevleri getir
    public async Task<List<AllTasksDto>> GetAllTasksAsync()
    {
        var entities = await _mongoDbService.GetAllTasksAsync();

        var dtos = entities
        .Select(item => new AllTasksDto
        {
            Id = item.Id.ToString(),
            Description = item.Description,
            Title = item.Title,
            IsCompleted = item.IsCompleted,
            CreatedAt = item.CreatedAt,
            EndDate = item.EndDate,
            UserId = item.UserId,
        })
        .ToList();

        return dtos;
    }

    // Görev ID'ye göre getir
    public async Task<SingleTaskDto> GetTaskByIdAsync(string id)  // ID'yi string (ObjectId) türüyle alıyoruz
    {
        var task = await _mongoDbService.GetTaskByIdAsync(id);  // MongoDbService üzerinden görev bilgilerini al

        if(task is null)
        {
            return null;
        }

        var dto = new SingleTaskDto
        {
            Description = task.Description,
            Title = task.Title,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            EndDate = task.EndDate,
            UserId = task.UserId,
            TaskUserName = task.User?.Username,
            Id = id
        };

        return dto;
    }

    // Yeni görev oluştur
    public async Task AddTaskAsync(AddTaskDto dto)
    {
        if(!string.IsNullOrEmpty(dto.UserId))
        {
            var user = await _mongoDbService.Users.Find(u => u.Id == new ObjectId(dto.UserId)).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("User not found");  // Kullanıcı bulunamazsa hata fırlat
            }
        }

        var entity = new TaskEntity
        {
            Description = dto.Description,
            Title = dto.Title,
            EndDate = dto.EndDate,
            UserId = dto.UserId,
        };

        await _mongoDbService.CreateTaskAsync(entity);
    }

    // Var olan görevi güncelle
    public async Task UpdateTaskAsync(string id, TaskEntity updatedTask)  // ID'yi string (ObjectId) olarak alıyoruz
    {
        var existingTask = await _mongoDbService.GetTaskByIdAsync(id);  // Mevcut görevi al
        if (existingTask == null)
        {
            throw new Exception("Task not found");  // Görev bulunamazsa hata fırlat
        }

        updatedTask.Id = existingTask.Id;  // Güncellenmiş görevde eski ID'yi koru
        await _mongoDbService.UpdateTaskAsync(id, updatedTask);  // MongoDbService üzerinden görevi güncelle
    }

    // Görev sil
    public async Task DeleteTaskAsync(string id)  // ID'yi string (ObjectId) olarak alıyoruz
    {
        var existingTask = await _mongoDbService.GetTaskByIdAsync(id);  // Mevcut görevi al
        if (existingTask == null)
        {
            throw new Exception("Task not found");  // Görev bulunamazsa hata fırlat
        }

        await _mongoDbService.DeleteTaskAsync(id);  // MongoDbService üzerinden görevi sil
    }

    // Kullanıcıya göre görevleri listele
    public async Task<List<TaskEntity>> GetTasksByUserIdAsync(string userId)  // UserId'yi string (ObjectId) olarak alıyoruz
    {
        var tasks = await _mongoDbService.GetAllTasksAsync();  // MongoDbService üzerinden tüm görevleri al
        return tasks.Where(task => task.UserId == userId).ToList();  // Kullanıcıya ait görevleri filtrele
    }
}