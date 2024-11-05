using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Functions.Reservations
{
    public class ConfirmReservation(
        IConfirmReservationHandler handler, ILogger<DraftApprenticeshipCreatedEvent> log) : IHandleMessages<DraftApprenticeshipCreatedEvent>
        {
            public async Task Handle(DraftApprenticeshipCreatedEvent message, IMessageHandlerContext context)
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

        //[FunctionName("ConfirmReservation")]
        //public static async Task Run([NServiceBusTrigger(EndPoint = QueueNames.ConfirmReservation)] DraftApprenticeshipCreatedEvent message, [Inject]ILogger<DraftApprenticeshipCreatedEvent> log, [Inject] IConfirmReservationHandler handler)
        //{
        //    log.LogInformation($"NServiceBus Confirm Reservation trigger function executed at: {DateTime.Now}");

        //    if (message.ReservationId.HasValue)
        //    {
        //        await handler.Handle(message);
        //        log.LogInformation($"Confirmed Reservation with ID: {message.ReservationId}");
        //    }
        //    else
        //    {
        //        log.LogInformation($"No reservation confirmed, no reservation ReservationId provided");
        //    }
        //}
    }
}
