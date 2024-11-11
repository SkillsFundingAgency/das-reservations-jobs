using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace SFA.DAS.Reservations.Functions.LegalEntities.Extensions;

public static class LoggingExtensions
{
    public static IServiceCollection AddDasLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(options =>
        {
            options.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            options.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);

            options.SetMinimumLevel(LogLevel.Trace);
            options.AddConsole();
            options.AddDebug();
        });
        
        return services;
    }
}