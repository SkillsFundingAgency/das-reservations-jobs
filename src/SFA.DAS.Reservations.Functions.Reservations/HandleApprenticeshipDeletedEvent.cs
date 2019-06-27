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

            try
            {
                await handler.Handle(message);
                log.LogInformation($"Updated Reservation with ID: {message.ReservationId}");
            }
            catch (Exception e)
            {
                log.LogError(e, e.Message, message);
            }
        }
    }
}
