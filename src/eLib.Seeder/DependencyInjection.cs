using eLib.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eLib.Seeder
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            services.AddDbContext<LibraryDbContext>((serviceProvider, opts) =>
            {
                opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<ISeeder, Seeder>();
        }
    }
}