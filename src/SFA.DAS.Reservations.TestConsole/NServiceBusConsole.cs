using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Configuration;

namespace SFA.DAS.Reservations.TestConsole
{
    public class NServiceBusConsole
    {
        public async Task Run()
        {
            var endpointConfiguration = new EndpointConfiguration(QueueNames.ConfirmReservation)
                .UseAzureServiceBusTransport(EnvironmentVariables.NServiceBusConnectionString, r => { })
                .UseErrorQueue()
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            var command = string.Empty;

            do
            {
                Console.WriteLine("Enter 'q' to exit..." + Environment.NewLine);
                Console.Write("Reservation ID: ");
                var guidStr = Console.ReadLine();

                var reservationId = Guid.Parse(guidStr);

                await endpointInstance.SendLocal(new ConfirmReservationMessage { ReservationId = reservationId });

                Console.WriteLine("Message sent...");
            } while (!command.Equals("q"));


            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
