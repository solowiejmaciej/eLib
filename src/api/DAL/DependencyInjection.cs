using Microsoft.EntityFrameworkCore;
using eLib.DAL.Repositories;

namespace eLib.DAL
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDAL(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
        }

        public static void AddAutomaticMigrations(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            context.Database.Migrate();
        }
    }
}