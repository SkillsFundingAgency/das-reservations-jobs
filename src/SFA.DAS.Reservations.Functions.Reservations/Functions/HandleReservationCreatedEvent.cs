using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations.Functions;

public class HandleReservationCreatedEvent(IReservationCreatedHandler handler,
    ILogger<ReservationCreatedEvent> log) : IHandleMessages<ReservationCreatedEvent>
{
    public async Task Handle(ReservationCreatedEvent message, IMessageHandlerContext context)
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