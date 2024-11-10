using System.Reflection;
using eLib;
using eLib.DAL;
using eLib.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddDAL(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddValidation(builder.Configuration);
builder.Services.AddMiddlewares();



builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();