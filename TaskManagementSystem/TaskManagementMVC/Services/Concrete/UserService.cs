namespace TaskManagementMVC.Services.Concrete;

using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementMVC.DTOs;
using TaskManagementMVC.Services.Abstract;
public class UserService : IUserService
{
    private readonly IHttpClientFactory _factory;
    public UserService(IHttpClientFactory factory)
    {
        _factory = factory;
    }
    private HttpClient UserApiClient => _factory.CreateClient("userApi");
    private HttpClient AuthApiClient => _factory.CreateClient("authApi");

    public async Task<ServiceResult> AddUserAsync(AddUserDto dto)
    {
        var apiResponse = await AuthApiClient.PostAsJsonAsync("create-user", dto);

        var result = await apiResponse.Content.ReadAsStringAsync();

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, result);
        }

        return new ServiceResult(false, result);
    }

    public async Task<ServiceResult> DeleteUserAsync(string userId)
    {
        var apiResponse = await UserApiClient.DeleteAsync($"delete/{userId}");

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, "Kullanıcı başarıyla silindi.");
        }

        return new ServiceResult(false, "Kullanıcı silinirken bir hata oluştu!..");
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

    public async Task<ServiceResult<UserDetailsDto>> GetUserDetailsAsync(string userId)
    {
        var userApiResponse = await UserApiClient.GetAsync(userId);

        if (userApiResponse.IsSuccessStatusCode)
        {
            var result = await userApiResponse.Content.ReadFromJsonAsync<UserDetailsDto>();

            if (result is null)
            {
                return new ServiceResult<UserDetailsDto>(false);
            }

            return new ServiceResult<UserDetailsDto>(true, null, result);
        }

        return new ServiceResult<UserDetailsDto>(false);
    }

    public async Task<ServiceResult> UpdateRoleAsync(UpdateRoleDto dto)
    {
        var apiResponse = await UserApiClient.PutAsJsonAsync("update-role", dto);

        var result = await apiResponse.Content.ReadAsStringAsync();

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, result);
        }

        return new ServiceResult(false, result);
    }

    public async Task<ServiceResult> UpdateUserAsync(UpdateUserDto dto)
    {
        var apiResponse = await UserApiClient.PutAsJsonAsync("update", dto);

        var result = await apiResponse.Content.ReadAsStringAsync();

        if (apiResponse.IsSuccessStatusCode)
        {
            return new ServiceResult(true, result);
        }

        return new ServiceResult(false, result);
    }
}
