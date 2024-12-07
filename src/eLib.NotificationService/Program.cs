using System.Reflection;
using eLib.Auth;
using eLib.Common;
using eLib.Events;
using eLib.NotificationService;
using eLib.NotificationService.Consumers;
using eLib.NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Configuration.AddAzureAppConfiguration();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddSwagger("eLib Notification Service", "v1");
builder.Services.AddConsuming(builder.Configuration, Assembly.GetExecutingAssembly());
builder.Services.AddCommon(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddServices(builder.Configuration);
builder.Services.UsePostgres(builder.Configuration);
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddAutomaticMigrations();
builder.Services.AddScoped<IPaginationService, PaginationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("corsPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
var app = builder.Build();

app.UseCors("corsPolicy");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();