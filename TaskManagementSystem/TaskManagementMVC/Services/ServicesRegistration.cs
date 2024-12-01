using TaskManagementMVC.Services.Abstract;
using TaskManagementMVC.Services.Concrete;

namespace TaskManagementMVC.Services;
public static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();

        var TaskApiUrl = configuration.GetValue<string>("TaskApiUrl");

        if (string.IsNullOrWhiteSpace(TaskApiUrl))
        {
            throw new InvalidOperationException("TaskApiUrl is required in appsettings.json");
        }

        services.AddHttpClient("taskApi", c =>
        {
            c.BaseAddress = new Uri(TaskApiUrl);
        });

        services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}