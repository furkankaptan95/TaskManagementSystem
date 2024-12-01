using TaskManagementMVC.DTOs;

namespace TaskManagementMVC.Services.Abstract;
public interface ITaskService
{
    Task<ServiceResult<List<AllTasksDto>>> GetAllAsync();
}
