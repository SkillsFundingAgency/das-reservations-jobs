using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SFA.DAS.Reservations.Functions.Reservations.Extensions;

public static class ServiceProviderExtensions
{
    public static ILogger GetLogger(this ServiceProvider serviceProvider, string typeName, IConfiguration configuration)
    {
        return configuration["EnvironmentName"] == "DEV"
            ? new NullLoggerFactory().CreateLogger(nameof(Startup))
            : serviceProvider.GetService<ILoggerProvider>().CreateLogger(typeName);
    }
}