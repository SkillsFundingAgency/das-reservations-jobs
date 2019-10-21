using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations
{
    public class HandleReservationDeletedEvent
    {
        [FunctionName("HandleReservationDeletedEvent")]
        public static async Task Run(
            [NServiceBusTrigger(EndPoint = QueueNames.ReservationDeleted)] ReservationDeletedEvent message,
            [Inject] ILogger<ReservationDeletedEvent> log,
            [Inject] INotifyEmployerWhenReservationDeletedAction action)
        {
            log.LogInformation($"Reservation Deleted function executing at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}].");

            await action.Execute<ReservationDeletedNotificationEvent>(message);

            log.LogInformation($"Reservation Deleted function finished at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}] has been handled.");
        }
    }
}