using Microsoft.EntityFrameworkCore;
using Todo.Core.Data;
using Todo.Core.Services;

var builder = WebApplication.CreateBuilder(args);

//  Prefer configuration-based access over environment variables
var connectionString = builder.Configuration["TODO_DATABASE_CONNECTION_STRING"];
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("The 'TODO_DATABASE_CONNECTION_STRING' configuration value is not set or is empty.");
}

//  Add DbContext with retry policy to handle transient startup errors
builder.Services.AddDbContext<AzureSqlDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
    c.RoutePrefix = string.Empty;
});

//  Run migrations on startup, with logging to diagnose cold start crashes
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AzureSqlDbContext>();
    dbContext.Database.Migrate();
}
catch (Exception ex)
{
    // Log startup errors (fallback to file if necessary)
    var logPath = Path.Combine(AppContext.BaseDirectory, "startup-error.log");
    await File.WriteAllTextAsync(logPath, ex.ToString());
    throw; // Let Azure show 500.30 so we know it failed
}

app.UseAuthorization();
app.MapControllers();
app.Run();
