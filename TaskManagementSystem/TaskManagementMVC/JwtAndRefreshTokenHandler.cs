namespace TaskManagementMVC;
public class JwtAndRefreshTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtAndRefreshTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null)
        {
            // Cookie'den JWT ve Refresh Token'ı al
            var jwtToken = httpContext.Request.Cookies["JwtToken"];
            var refreshToken = httpContext.Request.Cookies["RefreshToken"];

            if (!string.IsNullOrEmpty(jwtToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                request.Headers.Add("Refresh-Token", refreshToken);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
