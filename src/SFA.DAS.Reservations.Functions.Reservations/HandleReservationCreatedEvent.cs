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
            log.LogInformation($"Reservation Created function executing at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}].");

            if (message.Id != null && message.Id != Guid.Empty)
            {
                await handler.Handle(message);
                log.LogInformation($"Reservation Created function finished at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}] has been handled.");
            }
            else
            {
                log.LogInformation($"No reservation created, no reservation ReservationId provided");
            }
        }
    }
}