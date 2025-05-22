using System;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ConfirmReservationHandler(IReservationService service) : IConfirmReservationHandler
    {
        public async Task Handle(DraftApprenticeshipCreatedEvent draftApprenticeshipCreatedEvent)
        {
            if (!draftApprenticeshipCreatedEvent.ReservationId.HasValue || draftApprenticeshipCreatedEvent.ReservationId.Value == Guid.Empty)
            {
                throw new ArgumentException("ReservationId must be set", nameof(draftApprenticeshipCreatedEvent.ReservationId));
            }

            await service.UpdateReservationStatus(
                draftApprenticeshipCreatedEvent.ReservationId.Value, 
                ReservationStatus.Confirmed,
                draftApprenticeshipCreatedEvent.CreatedOn,
                draftApprenticeshipCreatedEvent.CohortId,
                draftApprenticeshipCreatedEvent.DraftApprenticeshipId);
        }
    }
}
