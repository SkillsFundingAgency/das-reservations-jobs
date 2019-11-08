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
    public class HandleReservationCreatedEvent
    {
        [FunctionName("HandleReservationCreatedEvent")]
        public static async Task Run(
            [NServiceBusTrigger(EndPoint = QueueNames.ReservationCreated)] ReservationCreatedEvent message,
            [Inject] ILogger<ReservationCreatedEvent> log,
            [Inject] INotifyEmployerOfReservationEventAction notifyAction,
            [Inject] IAddNonLevyReservationToReservationsIndexAction addNonLevyToReservationsIndexAction)
        {
            log.LogInformation($"Reservation Created function executing at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}].");

            await notifyAction.Execute<ReservationCreatedNotificationEvent>(message);
            await addNonLevyToReservationsIndexAction.Execute(message);

            log.LogInformation($"Reservation Created function finished at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}] has been handled.");
        }
    }
}