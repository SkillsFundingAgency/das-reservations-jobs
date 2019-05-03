using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Configuration;

namespace SFA.DAS.Reservations.TestConsole
{
    public class NServiceBusConsole
    {
        public async Task Run()
        {
            var configuration = new EndpointConfiguration(QueueNames.ConfirmReservation);
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.EnableInstallers();

            if (!string.IsNullOrEmpty(EnvironmentVariables.NServiceBusLicense))
            {
                configuration.License(EnvironmentVariables.NServiceBusLicense);
            }

            var serialization = configuration.UseSerialization<NewtonsoftSerializer>();
            serialization.WriterCreator(s => new JsonTextWriter(new StreamWriter(s, new UTF8Encoding(false))));

            var transport = configuration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(EnvironmentVariables.NServiceBusConnectionString);


            var endpointInstance = await Endpoint.Start(configuration)
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
