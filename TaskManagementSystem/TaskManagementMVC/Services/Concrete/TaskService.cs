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

    public async Task<ServiceResult> AssignTaskAsync(AssignTaskDto dto)
    {
        var apiResponse = await TaskApiClient.PutAsJsonAsync("assign", dto);

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, "Görev kullanıcıya başarıyla atandı.");
        }

        return new ServiceResult(false, "Görev atanırken bir hata oluştu!..");
    }

    public async Task<ServiceResult> DeleteTaskAsync(string taskId)
    {
        var apiResponse = await TaskApiClient.DeleteAsync($"delete/{taskId}");

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, "Görev başarıyla silindi.");
        }

        return new ServiceResult(false, "Görev silinirken bir hata oluştu!..");
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

    public async Task<ServiceResult> MarkAsCompletedAsync(string taskId)
    {
        var apiResponse = await TaskApiClient.PutAsJsonAsync("status",taskId);

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, "Görev durumu -Tamamlandı- olarak değiştirildi.");
        }

        return new ServiceResult(false, "Görev durumu değiştirilirken bir hata oluştu!..");
    }

    public async Task<ServiceResult> MarkAsOngoingAsync(string taskId)
    {
        var apiResponse = await TaskApiClient.PutAsJsonAsync("status", taskId);

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, "Görev durumu -Devam Ediyor- olarak değiştirildi.");
        }

        return new ServiceResult(false, "Görev durumu değiştirilirken bir hata oluştu!..");
    }

    public async Task<ServiceResult> ReplyQuestionAsync(ReplyQuestionDto dto)
    {
        var apiResponse = await TaskApiClient.PutAsJsonAsync("reply-question", dto);

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, "Cevap başarıyla eklendi.");
        }

        return new ServiceResult(false, "Cevap eklenirken bir hata oluştu!..");
    }

    public async Task<ServiceResult> UpdateTaskAsync(UpdateTaskDto dto)
    {
        var apiResponse = await TaskApiClient.PutAsJsonAsync("update", dto);

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, "Görev başarıyla güncellendi.");
        }

        return new ServiceResult(false, "Görev güncellenirken bir hata oluştu!..");
    }
}
