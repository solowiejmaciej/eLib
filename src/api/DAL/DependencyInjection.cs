using Microsoft.EntityFrameworkCore;
using eLib.DAL.Repositories;
using eLib.Services;

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
            services.AddScoped<ITwoStepCodeRepository, TwoStepCodeRepository>();

            services.AddScoped<IPaginationService, PaginationService>();
        }

        public static void AddAutomaticMigrations(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            context.Database.Migrate();
        }
    }
}