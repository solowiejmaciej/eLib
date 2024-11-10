using System.Reflection;
using eLib.Middleware;
using eLib.Services;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace eLib
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBookService, BookService>();
        }

        public static void AddValidation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationAutoValidation();
        }

        public static void AddMiddlewares(this IServiceCollection services)
        {
            services.AddTransient<ErrorHandlingMiddleware>();
        }
    }
}