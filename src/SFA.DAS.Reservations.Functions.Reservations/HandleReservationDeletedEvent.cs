using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Functions.Reservations
{
    public class HandleReservationDeletedEvent(IReservationDeletedHandler handler,
        ILogger<ReservationDeletedEvent> log) : IHandleMessages<ReservationDeletedEvent>
    {
        public async Task Handle(ReservationDeletedEvent message, IMessageHandlerContext context)
        {
            log.LogInformation($"Reservation Deleted function executing at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}].");

            if (message.Id != null && message.Id != Guid.Empty)
            {
                await handler.Handle(message);
                log.LogInformation($"Reservation Deleted function finished at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}] has been handled.");
            }
            else
            {
                log.LogInformation($"No reservation deleted, no reservation ReservationId provided");
            }
        }

        //[FunctionName("HandleReservationDeletedEvent")]
        //public static async Task Run(
        //    [NServiceBusTrigger(EndPoint = QueueNames.ReservationDeleted)] ReservationDeletedEvent message,
        //    [Inject] ILogger<ReservationDeletedEvent> log,
        //    [Inject] IReservationDeletedHandler handler)
        //{
        //    log.LogInformation($"Reservation Deleted function executing at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}].");

        //    if (message.Id != null && message.Id != Guid.Empty)
        //    {
        //        await handler.Handle(message);
        //        log.LogInformation($"Reservation Deleted function finished at: [{DateTime.UtcNow}] UTC, event with ID: [{message.Id}] has been handled.");
        //    }
        //    else
        //    {
        //        log.LogInformation($"No reservation deleted, no reservation ReservationId provided");
        //    }
        //}
    }
}