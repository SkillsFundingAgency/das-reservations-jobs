using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.TestConsole
{
    public class NServiceBusConsole
    {
        public async Task Run()
        {
            var connectionString = NServiceBus.AzureFunction.Infrastructure.EnvironmentVariables.NServiceBusConnectionString;
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Reservations.TestConsole")
                .UseAzureServiceBusTransport(connectionString, r =>
                {
                    r.RouteToEndpoint(typeof(ConfirmReservationMessage), QueueNames.ConfirmReservation);
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
                Console.WriteLine("Enter 'q' to exit..." + Environment.NewLine);
                Console.Write("Reservation ID: ");
                var guidStr = Console.ReadLine();

                var reservationId = Guid.Parse(guidStr);

                await endpointInstance.Send(new ConfirmReservationMessage { ReservationId = reservationId });

                Console.WriteLine("Message sent...");
            } while (!command.Equals("q"));


            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
