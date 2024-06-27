using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Functions.Reservations.Extensions;

[ExcludeFromCodeCoverage]
public static class NServiceBusServiceRegistrations
{
    private const string EndpointName = "SFA.DAS.Reservations.Functions.Reservations";
    
    public static IServiceCollection AddNServiceBus(this IServiceCollection services, ReservationsJobs configuration)
    {
        return services
            .AddSingleton(p =>
            {
                var hostingEnvironment = p.GetService<IHostEnvironment>();

                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .UseErrorQueue($"{EndpointName}-errors")
                    .UseLicense(configuration.NServiceBusLicense)
                    .UseMessageConventions()
                    .UseNewtonsoftJsonSerializer();

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