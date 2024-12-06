using TaskManagementMVC.DTOs;

namespace TaskManagementMVC.Services.Abstract;
public interface ITaskService
{
    Task<ServiceResult<List<AllTasksDto>>> GetAllTasksAsync();
    Task<ServiceResult<List<AllUsersDto>>> GetAllUsersAsync();
    Task<ServiceResult> AddAsync(AddTaskDto dto);
    Task<ServiceResult<SingleTaskDto>> GetSingleTaskAsync(string taskId);
    Task<ServiceResult> AddQuestionAsync(AddQuestionDto dto);
    Task<ServiceResult> ReplyQuestionAsync(ReplyQuestionDto dto);
    Task<ServiceResult> MarkAsCompletedAsync(string taskId);
    Task<ServiceResult> MarkAsOngoingAsync(string taskId);
}
