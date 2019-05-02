using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.TestConsole
{
    class NServiceBusConsole
    {
        public async Task Run()
        {
            var configuration = new EndpointConfiguration("testEndpoint");
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.EnableInstallers();

            var serialization = configuration.UseSerialization<NewtonsoftSerializer>();
            serialization.WriterCreator(s => new JsonTextWriter(new StreamWriter(s, new UTF8Encoding(false))));

            var transport = configuration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(Environment.GetEnvironmentVariable("NServiceBusConnectionString"));


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
