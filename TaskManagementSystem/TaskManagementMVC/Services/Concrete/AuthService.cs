using System.Net;
using TaskManagementMVC.DTOs;
using TaskManagementMVC.Services.Abstract;

namespace TaskManagementMVC.Services.Concrete;
public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _factory;
    public AuthService(IHttpClientFactory factory)
    {
        _factory = factory;
    }
    private HttpClient AuthApiClient => _factory.CreateClient("authApi");

    public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var response = await AuthApiClient.PostAsJsonAsync("forgot-password", forgotPasswordDto);
        if (response.IsSuccessStatusCode)
        {
            var successMessage = await response.Content.ReadAsStringAsync();
            return new ServiceResult(true, successMessage);
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        return new ServiceResult(false, errorMessage);
    }

    public async Task<ServiceResult<TokensDto>> LoginAsync(LoginDto loginDto)
    {
        var apiResponse = await AuthApiClient.PostAsJsonAsync("login", loginDto);

        if (apiResponse.IsSuccessStatusCode)
        {
            var successResult = await apiResponse.Content.ReadFromJsonAsync<ServiceResult<TokensDto>>();

            if (successResult is null)
            {
                return new ServiceResult<TokensDto>(false,"Something wrong happened.");
            }

            return successResult;
        }

        var result = await apiResponse.Content.ReadAsStringAsync();

        return new ServiceResult<TokensDto>(false,result);
    }

    public async Task<ServiceResult> NewPasswordAsync(NewPasswordDto dto)
    {
        var response = await AuthApiClient.PostAsJsonAsync("new-password", dto);

        var resultMessage = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return new ServiceResult(true, resultMessage);
        }

        return new ServiceResult(false, resultMessage);
    }

    public async Task<ServiceResult<TokensDto>> RefreshTokenAsync(string token)
    {
        var response = await AuthApiClient.PostAsJsonAsync("refresh-token", token);

        if (response.IsSuccessStatusCode)
        {
            var successResult = await response.Content.ReadFromJsonAsync<ServiceResult<TokensDto>>();

            if (successResult is null)
            {
                return new ServiceResult<TokensDto>(false);
            }

            return successResult;
        }

        var result = await response.Content.ReadAsStringAsync();

        return new ServiceResult<TokensDto>(false,result);
    }

    public async Task<ServiceResult<string>> RenewPasswordEmailAsync(RenewPasswordDto dto)
    {
          var response = await AuthApiClient.PostAsJsonAsync("renew-password-verify", dto);
          var result = await response.Content.ReadFromJsonAsync<ServiceResult<string>>();

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResult<string>(false,result.Message);
            }

        return result;
    }

    public async Task<ServiceResult> RevokeTokenAsync(string token)
    {
        try
        {
            var response = await AuthApiClient.PostAsJsonAsync("revoke-token", token);

            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ServiceResult(true,"Hesabınızdan başarıyla çıkış yapıldı.");
            }

            return new ServiceResult(false,"Hesabınızdan çıkış yapılırken bir problemle karşılaşıldı.");
        }
        catch (Exception)
        {
            return new ServiceResult(false, "Hesabınızdan çıkış yapılırken bir problemle karşılaşıldı.");
        }
    }
}
