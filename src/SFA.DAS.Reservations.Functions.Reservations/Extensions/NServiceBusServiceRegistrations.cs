using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.NServiceBus.AzureFunction.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Functions.Reservations.Extensions;

[ExcludeFromCodeCoverage]
public static class NServiceBusServiceRegistrations
{
    public static IServiceCollection AddNServiceBus(this IServiceCollection services, ReservationsJobs config, ILogger logger)
    {
        if (config.NServiceBusConnectionString.Equals("UseDevelopmentStorage=true", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddNServiceBus(logger, (options) =>
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
            services.AddNServiceBus(logger);
        }

        return services;
    }
    
    private static IServiceCollection AddNServiceBus(this IServiceCollection services, ILogger logger, Action<NServiceBusOptions> onConfigureOptions = null)
    {
        services.AddSingleton<IExtensionConfigProvider, NServiceBusExtensionConfigProvider>(provider =>
        {
            var options = new NServiceBusOptions
            {
                OnMessageReceived = context =>
                {
                    context.Headers.TryGetValue("NServiceBus.EnclosedMessageTypes", out string messageType);
                    context.Headers.TryGetValue("NServiceBus.MessageId", out string messageId);
                    context.Headers.TryGetValue("NServiceBus.CorrelationId", out string correlationId);
                    context.Headers.TryGetValue("NServiceBus.OriginatingEndpoint", out string originatingEndpoint);
                    logger.LogInformation($"Received NServiceBusTriggerData Message of type '{(messageType != null ? messageType.Split(',')[0] : string.Empty)}' with messageId '{messageId}' and correlationId '{correlationId}' from endpoint '{originatingEndpoint}'");
                },
                OnMessageErrored = (ex, context) =>
                {
                    context.Headers.TryGetValue("NServiceBus.EnclosedMessageTypes", out string messageType);
                    context.Headers.TryGetValue("NServiceBus.MessageId", out string messageId);
                    context.Headers.TryGetValue("NServiceBus.CorrelationId", out string correlationId);
                    context.Headers.TryGetValue("NServiceBus.OriginatingEndpoint", out string originatingEndpoint);
                    logger.LogError(ex, $"Error handling NServiceBusTriggerData Message of type '{(messageType != null ? messageType.Split(',')[0] : string.Empty)}' with messageId '{messageId}' and correlationId '{correlationId}' from endpoint '{originatingEndpoint}'");
                }
            };

            onConfigureOptions?.Invoke(options);

            return new NServiceBusExtensionConfigProvider(options);
        });

        return services;
    }
}