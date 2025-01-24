using TaskAPI.DTOs;

namespace TaskAPI.Services;

public interface ITaskService
{
    Task<List<AllTasksDto>> GetAllTasksAsync();
    Task<ServiceResult<SingleTaskDto>> GetTaskByIdAsync(string id);
    Task<ServiceResult> AddTaskAsync(AddTaskDto dto);
    Task<ServiceResult> UpdateTaskAsync(UpdateTaskDto dto);
    Task<ServiceResult> ChangeStatusAsync(string id);
    Task DeleteTaskAsync(string id);
    Task<List<AllTasksDto>> GetTasksByUserIdAsync(string userId);
    Task<ServiceResult> AddQuestionAsync(AddQuestionDto dto);
    Task<ServiceResult> ReplyQuestionAsync(ReplyQuestionDto dto);
    Task<ServiceResult> AssignTaskToUserAsync(AssignTaskDto dto);
}