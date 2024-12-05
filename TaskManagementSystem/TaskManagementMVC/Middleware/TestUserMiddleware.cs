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
            new Claim(ClaimTypes.Role, "Admin"), // Kullanıcı rolü
            new Claim(ClaimTypes.NameIdentifier, "674db629310f4842afafed6d") // Kullanıcı ID'si
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        context.User = new ClaimsPrincipal(identity);

        // Bir sonraki middleware'e devam et
        await _next(context);
    }
}
