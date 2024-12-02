using TaskManagementMVC.DTOs;

namespace TaskManagementMVC.Services.Abstract;
public interface ITaskService
{
    Task<ServiceResult<List<AllTasksDto>>> GetAllTasksAsync();
    Task<ServiceResult<List<AllUsersDto>>> GetAllUsersAsync();
    Task<ServiceResult> AddAsync(AddTaskDto dto);
}
