using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Extensions;

public static class LoggingExtensions
{
    public static IServiceCollection AddDasLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
                
            builder.SetMinimumLevel(LogLevel.Information);
                
            builder.AddConsole();
            builder.AddDebug();
        });
        
        return services;
    }    
}