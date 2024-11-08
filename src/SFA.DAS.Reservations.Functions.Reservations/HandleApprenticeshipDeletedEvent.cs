using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Functions.Reservations;

public class HandleApprenticeshipDeletedEvent(IApprenticeshipDeletedHandler handler, 
    ILogger<DraftApprenticeshipDeletedEvent> log) : IHandleMessages<DraftApprenticeshipDeletedEvent>
{
    public async Task Handle(DraftApprenticeshipDeletedEvent message, IMessageHandlerContext context)
    {
        log.LogInformation($"NServiceBus Apprenticeship Deleted trigger function executed at: {DateTime.Now}");

        if (message.ReservationId.HasValue)
        {
            await handler.Handle(message.ReservationId.Value);
            log.LogInformation($"Set Reservation with ID: {message.ReservationId} to pending");
        }
        else
        {
            log.LogInformation($"No reservation set to pending, no reservation ReservationId provided");
        }
    }
}
