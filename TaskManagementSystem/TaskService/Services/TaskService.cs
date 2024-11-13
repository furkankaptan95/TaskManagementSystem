using MongoDB.Driver;
using MongoDB.Bson;
using TaskAPI.Entities;

namespace TaskAPI.Services;

public class TaskService
{
    private readonly MongoDbService _mongoDbService;

    public TaskService(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    // Tüm görevleri getir
    public async Task<List<TaskEntity>> GetAllTasksAsync()
    {
        return await _mongoDbService.GetAllTasksAsync();  // MongoDbService üzerinden tüm görevleri getir
    }

    // Görev ID'ye göre getir
    public async Task<TaskEntity> GetTaskByIdAsync(string id)  // ID'yi string (ObjectId) türüyle alıyoruz
    {
        var task = await _mongoDbService.GetTaskByIdAsync(id);  // MongoDbService üzerinden görev bilgilerini al

        if (task == null)
        {
            throw new Exception("Task not found");  // Görev bulunamazsa hata fırlat
        }

        return task;
    }

    // Yeni görev oluştur
    public async Task CreateTaskAsync(TaskEntity task)
    {
        // Kullanıcıyı kontrol et
        var user = await _mongoDbService.Users.Find(u => u.Id == new ObjectId(task.UserId)).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new Exception("User not found");  // Kullanıcı bulunamazsa hata fırlat
        }

        await _mongoDbService.CreateTaskAsync(task);  // MongoDbService üzerinden yeni görev oluştur
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
