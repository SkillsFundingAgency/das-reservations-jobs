using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using NLog.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure.Logging;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Extensions;

public static class LoggingExtensions
{
    public static IServiceCollection AddDasLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var nLogConfiguration = new NLogConfiguration();

        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
                
            builder.SetMinimumLevel(LogLevel.Information);
                
            builder.AddConsole();
            builder.AddDebug();
            nLogConfiguration.ConfigureNLog(configuration);
            builder.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });
        });
        
        return services;
    }    
}