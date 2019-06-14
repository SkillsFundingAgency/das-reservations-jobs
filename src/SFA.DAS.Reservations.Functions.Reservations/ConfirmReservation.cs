using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using NLog;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.Reservations
{
    public class ConfirmReservation
    {
        [FunctionName("ConfirmReservation")]
        public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.ConfirmReservation)] ConfirmReservationMessage message, ILogger log, [Inject] IConfirmReservationHandler handler)
        {
            log.Info($"NServiceBus Confirm Reservation trigger function executed at: {DateTime.Now}");

            await handler.Handle(message.ReservationId);

            log.Info($"Confirmed Reservation with ID: {message.ReservationId}");
        }
    }
}
