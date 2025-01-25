﻿using TaskManagementMVC.DTOs;
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
}
