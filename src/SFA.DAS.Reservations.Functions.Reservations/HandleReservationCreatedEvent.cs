using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations
{
    public class HandleReservationCreatedEvent
    {
        [FunctionName("HandleReservationCreatedEvent")]
        public static async Task Run(
            [NServiceBusTrigger(EndPoint = QueueNames.ReservationCreated)] ReservationCreatedEvent message,
            [Inject] ILogger<ReservationCreatedEvent> log,
            [Inject] IReservationCreatedHandler handler)
        {
            log.LogInformation($"NServiceBus Reservation Created function executed at: {DateTime.Now}");

            await handler.Handle(message);

            log.LogInformation($"Reservation with ID: {message.Id} has been created.");
        }
    }
}