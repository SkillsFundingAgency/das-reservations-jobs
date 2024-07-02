using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Functions.Reservations.Extensions;

public static class NServiceBusServiceRegistrations
{
    private const string EndpointName = "SFA.DAS.Reservations.Jobs";

    public static IServiceCollection AddNServiceBus(this IServiceCollection services)
    {
        return services
            .AddSingleton(p =>
            {
                var sp = services.BuildServiceProvider();
                var configuration = sp.GetService<IOptions<ReservationsJobs>>().Value;

                var hostingEnvironment = p.GetService<IHostingEnvironment>();

                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .UseErrorQueue()
                    .UseInstallers()
                    .UseMessageConventions()
                    .UseNewtonsoftJsonSerializer()
                    .UseNLogFactory();

                if (hostingEnvironment.IsDevelopment())
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
