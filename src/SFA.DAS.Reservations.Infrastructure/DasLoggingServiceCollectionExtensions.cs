using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace SFA.DAS.Reservations.Infrastructure;

public static class DasLoggingServiceCollectionExtensions
{
    public static IServiceCollection AddDasLogging(this IServiceCollection services, string @namespace)
    {
        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);

            builder.AddFilter(@namespace, LogLevel.Information);
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddConsole();
        });

        return services;
    }
}
