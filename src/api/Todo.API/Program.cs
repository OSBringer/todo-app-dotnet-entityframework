using Microsoft.EntityFrameworkCore;
using Todo.Core.Data;
using Todo.Core.Services;


var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("TODO_DATABASE_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("The environment variable 'TODO_DATABASE_CONNECTION_STRING' is not set or is empty.");
}

builder.Services.AddDbContext<AzureSqlDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
});


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
