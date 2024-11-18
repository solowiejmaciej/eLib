using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using eLib.Common.Helpers;

namespace eLib.Common
{
    public static class ServiceCollectionExtensions
    {
        public static IConfigurationBuilder AddAzureAppConfiguration(this IConfigurationBuilder builder)
        {
            if (EnvironmentHelper.IsProduction())
            {
                var connectionString = Environment.GetEnvironmentVariable("AzureAppConfig");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("AzureAppConfig connection string is not set in environment variables.");
                }

                builder.AddAzureAppConfiguration(options =>
                {
                    options.Connect(connectionString)
                        .UseFeatureFlags()
                        .Select(KeyFilter.Any);
                });
            }

            return builder;
        }

        public static void AddCommon(this IServiceCollection services, IConfiguration configuration)
        {
        }
    }
}