using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.ProviderRelationships.Messages.Events;
using SFA.DAS.ProviderRelationships.Types.Models;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.TestConsole
{
    public class NServiceBusConsole
    {
        public async Task Run()
        {
            var connectionString = NServiceBus.AzureFunction.Configuration.EnvironmentVariables.NServiceBusConnectionString;
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Reservations.TestConsole")
                .UseAzureServiceBusTransport(connectionString, r =>
                {
                    r.RouteToEndpoint(typeof(UpdatedPermissionsEvent), QueueNames.UpdatedProviderPermissions);
                })
                .UseErrorQueue()
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            var command = string.Empty;

            do
            {
                var message = new UpdatedPermissionsEvent(
                    10, 11, 12,
                    13, 14, Guid.NewGuid(),
                    "test@example.com", "Test",
                    "Tester", new HashSet<Operation>(),null, DateTime.Now);

                await endpointInstance.Publish(message);

                Console.WriteLine("Message sent...");

                Console.WriteLine("Enter 'q' to exit..." + Environment.NewLine);
                command = Console.ReadLine();
            } while (!command.Equals("q"));


            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
