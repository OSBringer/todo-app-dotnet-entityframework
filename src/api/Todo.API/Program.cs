using Microsoft.EntityFrameworkCore;
using Todo.Core.Data;
using Todo.Core.Services;


var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Starting Todo API...");
Console.WriteLine("Environment: {0}", Environment.GetEnvironmentVariable("TODO_DATABASE_CONNECTION_STRING"));

builder.Services.AddDbContext<AzureSqlDbContext>(options =>
         options.UseSqlServer(Environment.GetEnvironmentVariable("TODO_DATABASE_CONNECTION_STRING")));

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddControllers();

var app = builder.Build();

// run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AzureSqlDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.  

app.UseAuthorization();

app.MapControllers();

app.Run();
