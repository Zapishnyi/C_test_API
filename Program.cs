using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
DotNetEnv.Env.Load();

// Add services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register EF Core with PostgreSQL
var connectionString = DotNetEnv.Env.GetString("DATABASE_URL");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Register UserRepository
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

// Auto-create the users table on startup
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<UserRepository>();
    await repo.CreateTableIfNotExistsAsync();
    Console.WriteLine("Users table ready.");
}

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

Console.WriteLine("API is running at http://localhost:5000");
Console.WriteLine("Swagger docs at http://localhost:5000/openapi/v1.json");
Console.WriteLine("Endpoints:");
Console.WriteLine("  GET    /api/users       - List all users");
Console.WriteLine("  GET    /api/users/{id}  - Get user by ID");
Console.WriteLine("  POST   /api/users       - Create user");
Console.WriteLine("  PUT    /api/users/{id}  - Update user");
Console.WriteLine("  DELETE /api/users/{id}  - Delete user");

app.Run();
