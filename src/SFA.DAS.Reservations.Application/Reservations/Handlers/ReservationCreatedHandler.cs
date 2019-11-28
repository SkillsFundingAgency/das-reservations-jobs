using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ReservationCreatedHandler : IReservationCreatedHandler
    {
        private readonly IAddNonLevyReservationToReservationsIndexAction _addNonLevyReservationToReservationsIndexAction;
        private readonly INotifyEmployerOfReservationEventAction _notifyAction;
        public ReservationCreatedHandler(IAddNonLevyReservationToReservationsIndexAction addNonLevyReservationToReservationsIndexAction, INotifyEmployerOfReservationEventAction notifyAction)
        {
            _addNonLevyReservationToReservationsIndexAction = addNonLevyReservationToReservationsIndexAction;
            _notifyAction = notifyAction;
        }
        public async Task Handle(ReservationCreatedEvent createdEvent)
        {
            await _notifyAction.Execute<ReservationCreatedNotificationEvent>(createdEvent);
            await _addNonLevyReservationToReservationsIndexAction.Execute(createdEvent);
        }
    }
}