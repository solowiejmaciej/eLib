using System.Reflection;
using eLib.Auth;
using eLib.Common;
using eLib.Events;
using eLib.NotificationService;
using eLib.NotificationService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Configuration.AddAzureAppConfiguration();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddSwagger("eLib Notification Service", "v1");
builder.Services.AddConsuming(builder.Configuration, Assembly.GetExecutingAssembly());
builder.Services.AddCommon(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();