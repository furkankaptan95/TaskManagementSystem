using MongoDB.Driver;
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

    public async Task<ServiceResult> UpdateTaskAsync(UpdateTaskDto dto)  // ID'yi string (ObjectId) olarak alıyoruz
    {
        if (!ObjectId.TryParse(dto.Id, out _))
        {
            return new ServiceResult(false, "Invalid TaskId Format");
        }

        var existingTask = await _mongoDbService.GetTaskByIdAsync(dto.Id);  // Mevcut görevi al

        if (existingTask == null)
        {
            return new ServiceResult(false, "Task to update is not exist.");
        }

        if(dto.UserId is not null)
        {
            if (!ObjectId.TryParse(dto.UserId, out _))
            {
                return new ServiceResult(false, "Invalid UserId Format");
            }

            var user = await _mongoDbService.Users.Find(u => u.Id == new ObjectId(dto.UserId)).FirstOrDefaultAsync();

            if (user == null)
            {
                return new ServiceResult(false, "User not found.");
            }
        }

       

        var updatedTask = new TaskEntity
        {
            Id = ObjectId.Parse(dto.Id),
            Description = dto.Description,
            Title = dto.Title,
            EndDate = dto.EndDate,
            UserId = dto.UserId,
            IsCompleted = dto.IsCompleted,
        };

        await _mongoDbService.UpdateTaskAsync(dto.Id, updatedTask);  // MongoDbService üzerinden görevi güncelle

        return new ServiceResult(true, "Task updated successfuly.");
    }

    public async Task<ServiceResult> ChangeStatusAsync(string id)  // ID'yi string (ObjectId) olarak alıyoruz
    {
        var existingTask = await _mongoDbService.GetTaskByIdAsync(id);  // Mevcut görevi al

        if (existingTask == null)
        {
            return new ServiceResult(false, "Task to update is not exist.");
        }

        existingTask.IsCompleted = !existingTask.IsCompleted;

        await _mongoDbService.UpdateTaskAsync(id, existingTask);  // MongoDbService üzerinden görevi güncelle

        return new ServiceResult(true, "Task updated successfuly.");
    }

    public async Task DeleteTaskAsync(string id)  // ID'yi string (ObjectId) olarak alıyoruz
    {
        var existingTask = await _mongoDbService.GetTaskByIdAsync(id);  // Mevcut görevi al

        if (existingTask == null)
        {
            return;
        }

        await _mongoDbService.DeleteTaskAsync(id);  // MongoDbService üzerinden görevi sil
    }

    public async Task<List<AllTasksDto>> GetTasksByUserIdAsync(string userId)  // UserId'yi string (ObjectId) olarak alıyoruz
    {
        var tasks = await _mongoDbService.GetAllTasksAsync();  // MongoDbService üzerinden tüm görevleri al
        var userTasks = tasks.Where(task => task.UserId == userId).ToList();

        var dtos = userTasks
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

    public async Task<ServiceResult> AddQuestionAsync(AddQuestionDto dto)
    {
        if (!ObjectId.TryParse(dto.UserId, out _))
        {
            return new ServiceResult(false, "Invalid UserId Format");
        }

        var user = await _mongoDbService.Users.Find(u => u.Id == new ObjectId(dto.UserId)).FirstOrDefaultAsync();

        if (user == null)
        {
            return new ServiceResult(false, "User not found.");
        }

        if (!ObjectId.TryParse(dto.TaskId, out _))
        {
            return new ServiceResult(false, "Invalid TaskId Format");
        }

        var task = await _mongoDbService.Tasks.Find(u => u.Id == new ObjectId(dto.TaskId)).FirstOrDefaultAsync();

        if (task == null)
        {
            return new ServiceResult(false, "Task not found.");
        }

        var entity = new QuestionEntity
        {
            Content = dto.Content,
            TaskId = dto.TaskId,
            UserId = dto.UserId,
        };

        await _mongoDbService.AddQuestionAsync(entity);

        return new ServiceResult(true, "Question added successfully.");
    }

    public async Task<ServiceResult> ReplyQuestionAsync(ReplyQuestionDto dto)
    {
        if (!ObjectId.TryParse(dto.QuestionId, out _))
        {
            return new ServiceResult(false, "Invalid QuestionId Format");
        }

        var question = await _mongoDbService.Questions.Find(u => u.Id == new ObjectId(dto.QuestionId)).FirstOrDefaultAsync();

        if(question is null)
        {
            return new ServiceResult(false, "Question not found.");
        }

        question.Answer = dto.Reply;
        question.AnsweredAt = DateTime.UtcNow;

        await _mongoDbService.UpdateQuestionAsync(question.Id.ToString(),question);

        return new ServiceResult(true, "Answer added successfully.");
    }

    public async Task<ServiceResult> AssignTaskToUserAsync(AssignTaskDto dto)
    {

        var existingTask = await _mongoDbService.GetTaskByIdAsync(dto.TaskId);  // Mevcut görevi al

        existingTask.UserId = dto.UserId;

        await _mongoDbService.UpdateTaskAsync(dto.TaskId, existingTask);  // MongoDbService üzerinden görevi güncelle

        return new ServiceResult(true, "Task assigned to user successfuly.");
    }

}
