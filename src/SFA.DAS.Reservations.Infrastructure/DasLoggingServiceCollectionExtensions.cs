using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.Infrastructure;

public static class DasLoggingServiceCollectionExtensions
{
    public static IServiceCollection AddDasLogging(this IServiceCollection services, string @namespace)
    {
        services.AddLogging(builder =>
        {
            builder.AddFilter(@namespace, LogLevel.Information);
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddConsole();
        });

        return services;
    }
}
