using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;
using SFA.DAS.Reservations.Infrastructure.NServiceBus;

namespace SFA.DAS.Reservations.Functions.Reservations
{
    public class ConfirmReservation
    {
        [FunctionName("ConfirmReservation")]
        public static async Task Run([NServiceBusTrigger(QueueName = QueueNames.ConfirmReservation)] ConfirmReservationMessage message, ILogger log, [Inject] IConfirmReservationHandler handler)
        {
            log.LogInformation($"NServiceBus Confirm Reservation trigger function executed at: {DateTime.Now}");

            await handler.Handle(message.ReservationId);

            log.LogInformation($"Confirmed Reservation with ID: {message.ReservationId}");
        }
    }
}
