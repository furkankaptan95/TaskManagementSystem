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
            Username = item.User?.Username,
        })
        .ToList();

        return dtos;
    }

    public async Task<ServiceResult<SingleTaskDto>> GetTaskByIdAsync(string id)
    {
        var taskFilter = Builders<TaskEntity>.Filter.Eq(task => task.Id, new ObjectId(id));
        var task = await _mongoDbService.GetTaskAsync(taskFilter);

        if(task is null)
        {
            return new ServiceResult<SingleTaskDto>(false, "Task not found.");
        }

        var dto = new SingleTaskDto
        {
            Id = id,
            Description = task.Description,
            Title = task.Title,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            EndDate = task.EndDate,
            UpdatedAt = task.UpdatedAt,
            AssignedAt = task.AssignedAt,
            UserId = task.UserId,
            UserName = task.User?.Username,
        };

        if(dto.UserName is null)
        {
           return new ServiceResult<SingleTaskDto>(true, "No questions on this task.", dto);
        }

        var questionFilter = Builders<QuestionEntity>.Filter.Eq(q => q.TaskId, id);
        var questions = await _mongoDbService.GetTasksQuestionsAsync(questionFilter);

        var questionDtos = questions
            .Select(item => new QuestionDto
            {
                Id = item.Id.ToString(),
                Content = item.Content,
                Answer = item.Answer,
                CreatedAt = item.CreatedAt,
                AnsweredAt = item.CreatedAt,
                TaskId = item.TaskId,
                TaskTitle = task.Title,
                UserId = item.UserId,
                Username = dto.UserName
            }).ToList();

        dto.Questions = questionDtos;

        return new ServiceResult<SingleTaskDto>(true,null,dto);
    }

    public async Task<ServiceResult> AddTaskAsync(AddTaskDto dto)
    {
        if(dto.UserId is not null)
        {
            var userFilter = Builders<UserEntity>.Filter.Eq(u => u.Id ,new ObjectId(dto.UserId));
            var user = await _mongoDbService.GetUserAsync(userFilter);

            if (user is null)
            {
                return new ServiceResult(false, "User not found");
            }
        }

        var entity = new TaskEntity
        {
            Description = dto.Description,
            Title = dto.Title,
            EndDate = dto.EndDate,
            UserId = dto.UserId,
        };

        if(entity.UserId is not null)
        {
            entity.AssignedAt = DateTime.UtcNow;
        }

        await _mongoDbService.CreateTaskAsync(entity);

        return new ServiceResult(true, "Task created successfully.");
    }

    public async Task<ServiceResult> UpdateTaskAsync(UpdateTaskDto dto)  // ID'yi string (ObjectId) olarak alıyoruz
    {
        if (!ObjectId.TryParse(dto.Id, out _))
        {
            return new ServiceResult(false, "Invalid TaskId Format");
        }

        var taskFilter = Builders<TaskEntity>.Filter.Eq(task => task.Id, new ObjectId(dto.Id));
        var existingTask = await _mongoDbService.GetTaskAsync(taskFilter);  // Mevcut görevi al

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

        await _mongoDbService.UpdateTaskAsync(dto.Id, updatedTask);

        return new ServiceResult(true, "Task updated successfuly.");
    }

    public async Task<ServiceResult> ChangeStatusAsync(string id)
    {
        var taskFilter = Builders<TaskEntity>.Filter.Eq(task => task.Id, new ObjectId(id));
        var existingTask = await _mongoDbService.GetTaskAsync(taskFilter);

        if (existingTask == null)
        {
            return new ServiceResult(false, "Task to update is not exist.");
        }

        existingTask.IsCompleted = !existingTask.IsCompleted;

        await _mongoDbService.UpdateTaskAsync(id, existingTask);  // MongoDbService üzerinden görevi güncelle

        return new ServiceResult(true, "Task updated successfuly.");
    }

    public async Task DeleteTaskAsync(string id)
    {
        var taskFilter = Builders<TaskEntity>.Filter.Eq(task => task.Id, new ObjectId(id));
        var existingTask = await _mongoDbService.GetTaskAsync(taskFilter);

        if (existingTask == null)
        {
            return;
        }

        await _mongoDbService.DeleteTaskAsync(id);
    }

    public async Task<List<AllTasksDto>> GetTasksByUserIdAsync(string userId)
    {
        var tasks = await _mongoDbService.GetAllTasksAsync();
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
        var taskFilter = Builders<TaskEntity>.Filter.Eq(task => task.Id, new ObjectId(dto.TaskId));
        var existingTask = await _mongoDbService.GetTaskAsync(taskFilter);  // Mevcut görevi al

        existingTask.UserId = dto.UserId;

        await _mongoDbService.UpdateTaskAsync(dto.TaskId, existingTask);  // MongoDbService üzerinden görevi güncelle

        return new ServiceResult(true, "Task assigned to user successfuly.");
    }

}
