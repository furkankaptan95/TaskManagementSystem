using TaskAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mongoDbService = scope.ServiceProvider.GetRequiredService<MongoDbService>();
    // MongoDbService içinde veritabaný ve koleksiyonlar oluþturulacak.
}

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
