using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.Functions.Reservations.Extensions;

public static class ServiceProviderExtensions
{
    public static ILogger GetLogger(this ServiceProvider serviceProvider, string typeName) => serviceProvider.GetService<ILoggerProvider>().CreateLogger(typeName);
}