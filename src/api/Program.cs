using System.Reflection;
using eLib;
using eLib.DAL;
using eLib.Events;
using eLib.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddDAL(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddValidation(builder.Configuration);
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddMiddlewares();
builder.Services.AddPublishing(builder.Configuration);

builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();