using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ReservationDeletedHandler : IReservationDeletedHandler
    {
        private readonly IReservationService _reservationService;
        private readonly INotifyEmployerOfReservationEventAction _action;

        public ReservationDeletedHandler(IReservationService reservationService, INotifyEmployerOfReservationEventAction action)
        {
            _reservationService = reservationService;
            _action = action;
        }

        public async Task Handle(ReservationDeletedEvent deletedEvent, IMessageHandlerContext context)
        {
            await _action.Execute<ReservationDeletedNotificationEvent>(deletedEvent, context);
            await _reservationService.UpdateReservationStatus(deletedEvent.Id, ReservationStatus.Deleted);
        }
    }
}