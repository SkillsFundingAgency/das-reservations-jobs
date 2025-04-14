using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ReservationDeletedHandler(
        IReservationService reservationService,
        INotifyEmployerOfReservationEventAction action)
        : IReservationDeletedHandler
    {
        public async Task Handle(ReservationDeletedEvent deletedEvent, IMessageHandlerContext context)
        {
            await action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, context);
            await reservationService.UpdateReservationStatus(deletedEvent.Id, ReservationStatus.Deleted);
        }
    }
}