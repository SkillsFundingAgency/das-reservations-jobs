using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.Reservations
{
    public class ConfirmReservation
    {
        [FunctionName("ConfirmReservation")]
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.ConfirmReservation)] DraftApprenticeshipCreatedEvent message, [Inject]ILogger<DraftApprenticeshipCreatedEvent> log, [Inject] IConfirmReservationHandler handler)
        {
            log.LogInformation($"NServiceBus Confirm Reservation trigger function executed at: {DateTime.Now}");

            if (message.ReservationId.HasValue)
            {
                await handler.Handle(message);
                log.LogInformation($"Confirmed Reservation with ID: {message.ReservationId}");
            }
            else
            {
                log.LogInformation($"No reservation confirmed, no reservation ReservationId provided");
            }
        }
    }
}
