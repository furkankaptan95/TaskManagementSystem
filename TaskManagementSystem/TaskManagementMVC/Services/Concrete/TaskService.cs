using TaskManagementMVC.Services.Abstract;

namespace TaskManagementMVC.Services.Concrete;
public class TaskService : ITaskService
{
    private readonly IHttpClientFactory _factory;
    public TaskService(IHttpClientFactory factory)
    {
        _factory = factory;
    }
    private HttpClient ApiClient => _factory.CreateClient("api");
}
