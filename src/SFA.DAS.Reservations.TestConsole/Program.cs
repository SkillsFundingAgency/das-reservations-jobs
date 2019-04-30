using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        public static async Task Run()
        {
            var endpointConfiguration = new EndpointConfiguration("testQueue");

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            var command = string.Empty;

            do
            {
                Console.WriteLine("Enter 'q' to exit..." + Environment.NewLine);
                Console.Write("Reservation ID: ");
                var guidStr = Console.ReadLine();

                var reservationId = Guid.Parse(guidStr);

                await endpointInstance.SendLocal(new ConfirmReservationMessage {ReservationId = reservationId});

                Console.WriteLine("Message sent...");

            } while (!command.Equals("q"));
            

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
