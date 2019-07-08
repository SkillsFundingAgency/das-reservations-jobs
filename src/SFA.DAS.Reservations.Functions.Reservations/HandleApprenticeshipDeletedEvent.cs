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
    public class HandleApprenticeshipDeletedEvent
    {
        [FunctionName("HandleApprenticeshipDeletedEvent")]
        public static async Task Run(
            [NServiceBusTrigger(EndPoint = QueueNames.DraftApprenticeshipDeleted)] DraftApprenticeshipDeletedEvent message, 
            [Inject]ILogger<DraftApprenticeshipDeletedEvent> log, 
            [Inject] IApprenticeshipDeletedHandler handler)
        {
            log.LogInformation($"NServiceBus Apprenticeship Deleted trigger function executed at: {DateTime.Now}");

            if (message.ReservationId.HasValue)
            {
                await handler.Handle(message.ReservationId.Value);
                log.LogInformation($"Set Reservation with ID: {message.ReservationId} to pending");
            }
            else
            {
                log.LogInformation($"No reservation set to pending, no reservation Id provided");
            }

        }
    }
}
