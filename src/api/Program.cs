using System.Reflection;
using eLib;
using eLib.Auth;
using eLib.Common;
using eLib.DAL;
using eLib.Events;
using eLib.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Configuration.AddAzureAppConfiguration();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddDAL(builder.Configuration);
builder.Services.AddAutomaticMigrations();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddValidation(builder.Configuration);
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddSwagger("eLib API", "v1");
builder.Services.AddMiddlewares();
builder.Services.AddPublishing(builder.Configuration);
builder.Services.AddCommon(builder.Configuration);
builder.Services.AddHealthChecks();

builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddCors(options =>
{
    options.AddPolicy("corsPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Location");
    });
});
var app = builder.Build();

app.UseCors("corsPolicy");
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();