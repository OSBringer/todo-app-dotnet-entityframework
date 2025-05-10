using Microsoft.EntityFrameworkCore;
using Todo.Core.Data;
using Todo.Core.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AzureSqlDbContext>(options =>
         options.UseSqlServer(Environment.GetEnvironmentVariable("TODO_DATABASE_CONNECTION_STRING")));

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddControllers();

var app = builder.Build();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
