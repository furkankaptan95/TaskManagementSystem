using AuthAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// MongoDbService servisini ekliyoruz
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

var emailApiUrl = builder.Configuration.GetValue<string>("EmailApiUrl");

if (string.IsNullOrWhiteSpace(emailApiUrl))
{
    throw new InvalidOperationException("EmailApiUrl is required in appsettings.json");
}

builder.Services.AddHttpClient("emailApi", c =>
{
    c.BaseAddress = new Uri(emailApiUrl);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
