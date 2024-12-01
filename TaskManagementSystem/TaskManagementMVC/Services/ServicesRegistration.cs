namespace TaskManagementMVC.Services;
public static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();

        var apiUrl = configuration.GetValue<string>("ApiUrl");
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            throw new InvalidOperationException("ApiUrl is required in appsettings.json");
        }
        services.AddHttpClient("api", c =>
        {
            c.BaseAddress = new Uri(apiUrl);
        });

        return services;
    }
}