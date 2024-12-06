using System.Security.Claims;

namespace TaskManagementMVC.Middleware;
public class TestUserMiddleware
{
    private readonly RequestDelegate _next;

    public TestUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Test kullanıcısını oluştur
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "TestUser"), // Kullanıcı adı
            new Claim(ClaimTypes.Role, "User"), // Kullanıcı rolü
            new Claim(ClaimTypes.NameIdentifier, "673f990fff722f80c23c27a3") // Kullanıcı ID'si
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        context.User = new ClaimsPrincipal(identity);

        // Bir sonraki middleware'e devam et
        await _next(context);
    }
}
