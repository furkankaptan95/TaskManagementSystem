using TaskManagementMVC.Services.Abstract;
using TaskManagementMVC.Services.Concrete;

namespace TaskManagementMVC.Services;
public static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
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

        return services;

    }
}