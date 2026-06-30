using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Repositories;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var connectionString = DotNetEnv.Env.GetString("DATABASE_URL");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<UserRepository>();
    Console.WriteLine("Users table ready.");
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "api-docs";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API");
});

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();
}

app.MapControllers();

Console.WriteLine("API is running at http://localhost:5000");
Console.WriteLine("Swagger docs at http://localhost:5000/api-docs");

app.Run();
