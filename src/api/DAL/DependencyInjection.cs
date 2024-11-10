using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eLib.DAL.Repositories;
using eLib.DomainEvents;
using eLib.DomainEvents.Handlers;
using eLib.DomainEvents.Handlers.Book;

namespace eLib.DAL
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBookRepository, BookRepository>();

            return services;
        }
    }
}