using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserAPI.Helpers;

namespace UserAPI.Services;
public static class ServicesRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

        // MongoDbService servisini ekliyoruz
        services.AddSingleton<MongoDbService>();
        services.AddSingleton<RabbitMQConnectionHelper>();
        services.AddScoped<RabbitMQProducer>();


        AddJwtAuth(services, configuration);
        // UserService'i ekliyoruz
        services.AddScoped<IUserService, UserService>();

        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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
                    var accessToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },

                OnChallenge = async context =>
                {
                    // Token geçersiz olduğu için 401 döndürüyoruz
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\": \"Unauthorized. Please login.\"}");

                    context.HandleResponse(); // Yanıt tamamlandı, isteği durdur
                },

                OnForbidden = context =>
                {
                    // Yetki sorunu varsa 403 Forbidden dön
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Forbidden. You do not have access to this resource.\"}");
                }
            };
        });
    }
}
