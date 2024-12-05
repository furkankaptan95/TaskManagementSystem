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
    private HttpClient UserApiClient => _factory.CreateClient("userApi");

    public async Task<ServiceResult> AddAsync(AddTaskDto dto)
    {
        var apiResponse = await TaskApiClient.PostAsJsonAsync("add",dto);

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true,"Yeni Görev başarıyla oluşturuldu.");
        }

        return new ServiceResult(false, "Görevi atamak istediğiniz kullanıcı bulunamadığı için görev oluşturulamadı!..");
    }

    public async Task<ServiceResult> AddQuestionAsync(AddQuestionDto dto)
    {
        var apiResponse = await TaskApiClient.PostAsJsonAsync("add-question", dto);

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, "Soru başarıyla eklendi.");
        }

        return new ServiceResult(false, "Soru eklenirken bir hata oluştu!..");
    }

    public async Task<ServiceResult<List<AllTasksDto>>> GetAllTasksAsync()
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

    public async Task<ServiceResult<List<AllUsersDto>>> GetAllUsersAsync()
    {
        var userApiResponse = await UserApiClient.GetAsync("all");

        if (userApiResponse.IsSuccessStatusCode)
        {
            var result = await userApiResponse.Content.ReadFromJsonAsync<List<AllUsersDto>>();

            if (result is null)
            {
                return new ServiceResult<List<AllUsersDto>>(false);
            }

            return new ServiceResult<List<AllUsersDto>>(true, null, result);
        }

        return new ServiceResult<List<AllUsersDto>>(false);
    }

    public async Task<ServiceResult<SingleTaskDto>> GetSingleTaskAsync(string taskId)
    {
        var apiResponse = await TaskApiClient.GetAsync(taskId);

        if (apiResponse.IsSuccessStatusCode)
        {
            var result = await apiResponse.Content.ReadFromJsonAsync<SingleTaskDto>();

            if (result is null)
            {
                return new ServiceResult<SingleTaskDto>(false);
            }

            return new ServiceResult<SingleTaskDto>(true, null, result);
        }

        return new ServiceResult<SingleTaskDto>(false);
    }

    public async Task<ServiceResult> ReplyQuestionAsync(ReplyQuestionDto dto)
    {
        var apiResponse = await TaskApiClient.PostAsJsonAsync("reply-question", dto);

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, "Cevap başarıyla eklendi.");
        }

        return new ServiceResult(false, "Cevap eklenirken bir hata oluştu!..");
    }
}
