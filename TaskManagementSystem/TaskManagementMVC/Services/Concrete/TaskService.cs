using TaskManagementMVC.DTOs;
using TaskManagementMVC.Services.Abstract;

namespace TaskManagementMVC.Services.Concrete;
public class TaskService : ITaskService
{
    private readonly IHttpClientFactory _factory;
    public TaskService(IHttpClientFactory factory)
    {
        _factory = factory;
    }
    private HttpClient TaskApiClient => _factory.CreateClient("taskApi");

    public async Task<ServiceResult<List<AllTasksDto>>> GetAllAsync()
    {
        var apiResponse = await TaskApiClient.GetAsync("all");

        if (apiResponse.IsSuccessStatusCode)
        {
            var result = await apiResponse.Content.ReadFromJsonAsync<List<AllTasksDto>>();

            if(result is null)
            {
                return new ServiceResult<List<AllTasksDto>>(false);
            }

            return new ServiceResult<List<AllTasksDto>>(true,null,result);
        }

        return new ServiceResult<List<AllTasksDto>>(false);
    }
}
