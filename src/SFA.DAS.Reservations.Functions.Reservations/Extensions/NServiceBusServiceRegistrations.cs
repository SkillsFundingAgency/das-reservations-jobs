using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.NServiceBus.AzureFunction.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Functions.Reservations.Extensions;

[ExcludeFromCodeCoverage]
public static class NServiceBusServiceRegistrations
{
    public static IServiceCollection AddNServiceBus(this IServiceCollection services, ReservationsJobs config)
    {
        if (config.NServiceBusConnectionString.Equals("UseDevelopmentStorage=true", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddNServiceBus(options =>
            {
                options.EndpointConfiguration = endpoint =>
                {
                    endpoint.UseTransport<LearningTransport>().StorageDirectory(
                        Path.Combine(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("src")), @"src\.learningtransport"));
                    
                    return endpoint;
                };
            });
        }
        else
        {
            Environment.SetEnvironmentVariable("NServiceBusConnectionString", config.NServiceBusConnectionString);
            services.AddNServiceBus();
        }

        return services;
    }
    
    private static IServiceCollection AddNServiceBus(this IServiceCollection services, Action<NServiceBusOptions> onConfigureOptions = null)
    {
        services.AddSingleton<IExtensionConfigProvider, NServiceBusExtensionConfigProvider>(provider =>
        {
            var options = new NServiceBusOptions();
            
            onConfigureOptions?.Invoke(options);

            return new NServiceBusExtensionConfigProvider(options);
        });

        return services;
    }
}