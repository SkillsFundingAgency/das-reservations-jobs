using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using NLog.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure.Logging;

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
            options.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });
            options.AddConsole();
            options.AddDebug();

            var nLogConfiguration = new NLogConfiguration();
            
            nLogConfiguration.ConfigureNLog(configuration);
        });
        
        return services;
    }
}