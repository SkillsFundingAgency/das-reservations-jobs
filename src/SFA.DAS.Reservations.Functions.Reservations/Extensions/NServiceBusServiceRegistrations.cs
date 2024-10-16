using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Functions.Reservations.Extensions;

public static class NServiceBusServiceRegistrations
{
    private const string EndpointName = "SFA.DAS.Reservations.Jobs";

    public static IServiceCollection AddNServiceBus(this IServiceCollection services, string environmentName)
    {
        return services
            .AddSingleton(p =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetService<IOptions<ReservationsJobs>>().Value;
                
                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .UseErrorQueue()
                    .UseLicense(configuration.NServiceBusLicense)
                    .UseInstallers()
                    .UseMessageConventions()
                    .UseNewtonsoftJsonSerializer()
                    .UseNLogFactory();

                if (environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
                {
                    endpointConfiguration.UseLearningTransport(s => s.AddRouting());
                }
                else
                {
                    endpointConfiguration.UseAzureServiceBusTransport(configuration.NServiceBusConnectionString, s => s.AddRouting());
                }

                return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            })
            .AddSingleton<IMessageSession>(s => s.GetService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();
    }
}

public static class RoutingSettingsExtensions
{
    private const string NotificationsMessageHandler = "SFA.DAS.Notifications.MessageHandlers";

    public static void AddRouting(this RoutingSettings routingSettings)
    {
        routingSettings.RouteToEndpoint(typeof(SendEmailCommand), NotificationsMessageHandler);
    }
}
