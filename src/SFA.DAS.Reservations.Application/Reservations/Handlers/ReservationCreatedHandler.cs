using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ReservationCreatedHandler(
        IAddNonLevyReservationToReservationsIndexAction addNonLevyReservationToReservationsIndexAction,
        INotifyEmployerOfReservationEventAction notifyAction)
        : IReservationCreatedHandler
    {
        public async Task Handle(ReservationCreatedEvent createdEvent, IMessageHandlerContext context)
        {
            await notifyAction.Execute<ReservationCreatedNotificationEvent>(createdEvent, context);
            await addNonLevyReservationToReservationsIndexAction.Execute(createdEvent);
        }
    }
}