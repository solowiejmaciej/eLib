namespace eLib.Common.Helpers;

public static class EnvironmentHelper
{
    public static bool IsProduction()
    {
       return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
    }

    public static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }

    public static bool IsStaging()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Staging";
    }
}