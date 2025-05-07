using Microsoft.EntityFrameworkCore;
using Todo.Core.Data;
using Todo.Core.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  
builder.Services.AddDbContextPool<AzureSqlDbContext>(options => options
              .UseSqlServer(builder.Configuration.GetConnectionString("TodoDatabase")));
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
