using Microsoft.EntityFrameworkCore;
using MyApp.Config;
using MyApp.Data;
using MyApp.Repositories;

// Load and initialize AppConfig singleton (loads .env file internally)
_ = AppConfig.Instance;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(AppConfig.Instance.DatabaseUrl)
);

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PostRepository>();

var app = builder.Build();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (await db.Database.CanConnectAsync())
    {
        Console.WriteLine("Database connection successful.");
        if (app.Environment.IsDevelopment())
        {
            await db.Database.MigrateAsync();
            Console.WriteLine("Database migrations applied.");
        }
    }
    else
    {
        Console.WriteLine("Cannot connect to database.");
        return;
    }
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "api-docs";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API");
});

Console.WriteLine("API is running at http://localhost:5000");
Console.WriteLine("Swagger docs at http://localhost:5000/api-docs");

app.Run();
