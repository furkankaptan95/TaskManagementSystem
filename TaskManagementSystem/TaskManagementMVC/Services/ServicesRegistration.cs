using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagementMVC.Services.Abstract;
using TaskManagementMVC.Services.Concrete;

namespace TaskManagementMVC.Services;
public static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddJwtAuth(services, configuration);
        services.AddControllersWithViews();

        var taskApiUrl = configuration.GetValue<string>("TaskApiUrl");

        if (string.IsNullOrWhiteSpace(taskApiUrl))
        {
            throw new InvalidOperationException("TaskApiUrl is required in appsettings.json");
        }

        services.AddHttpClient("taskApi", c =>
        {
            c.BaseAddress = new Uri(taskApiUrl);
        });

        var userApiUrl = configuration.GetValue<string>("UserApiUrl");

        if (string.IsNullOrWhiteSpace(userApiUrl))
        {
            throw new InvalidOperationException("UserApiUrl is required in appsettings.json");
        }

        services.AddHttpClient("userApi", c =>
        {
            c.BaseAddress = new Uri(userApiUrl);
        });

        var authApiUrl = configuration.GetValue<string>("AuthApiUrl");

        if (string.IsNullOrWhiteSpace(authApiUrl))
        {
            throw new InvalidOperationException("AuthApiUrl is required in appsettings.json");
        }

        services.AddHttpClient("authApi", c =>
        {
            c.BaseAddress = new Uri(authApiUrl);
        });

        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;

    }

    private static void AddJwtAuth(IServiceCollection services, IConfiguration configuration)
    {
        var tokenOptions = configuration.GetSection("Jwt").Get<JwtTokenOptions>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = tokenOptions.Issuer,
                ValidateIssuer = true,
                ValidAudience = tokenOptions.Audience,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.Key)),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };

            options.MapInboundClaims = true;

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Cookies["JwtToken"];

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;

                    }

                    return Task.CompletedTask;
                },


                OnChallenge = async context =>
                {
                    var refreshToken = context.Request.Cookies["RefreshToken"];

                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                        var result = await authService.RefreshTokenAsync(refreshToken);

                        if (result.IsSuccess)
                        {
                            context.Response.Cookies.Append("JwtToken", result.Data.JwtToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                Expires = DateTime.UtcNow.AddMinutes(10)
                            });

                            context.Response.Cookies.Append("RefreshToken", result.Data.RefreshToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                Expires = DateTime.UtcNow.AddDays(7)
                            });

                            context.HandleResponse();
                            context.HttpContext.Response.Redirect(context.Request.Path.Value);
                            return;
                        }
                    }

                    context.Response.Redirect("/Auth/Login");
                    context.HandleResponse();
                },

                OnForbidden = context =>
                {
                    // Yetki sorunu varsa, MVC'ye yönlendirme yap
                    context.Response.Redirect("/Auth/Forbidden"); // MVC'de Forbidden sayfasına yönlendir
                    return Task.CompletedTask; // Yönlendirme sonrası işlemi sonlandır
                }
            };
        });
    }
}