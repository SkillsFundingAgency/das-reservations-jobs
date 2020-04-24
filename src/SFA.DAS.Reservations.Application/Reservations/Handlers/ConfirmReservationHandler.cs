using System;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ConfirmReservationHandler : IConfirmReservationHandler
    {
        private readonly IReservationService _service;

        public ConfirmReservationHandler(IReservationService service)
        {
            _service = service;
        }

        public async Task Handle(DraftApprenticeshipCreatedEvent draftApprenticeshipCreatedEvent)
        {
            if (!draftApprenticeshipCreatedEvent.ReservationId.HasValue || draftApprenticeshipCreatedEvent.ReservationId.Value == Guid.Empty)
            {
                throw new ArgumentException("ReservationId must be set", nameof(draftApprenticeshipCreatedEvent.ReservationId));
            }

            await _service.UpdateReservationStatus(
                draftApprenticeshipCreatedEvent.ReservationId.Value, 
                ReservationStatus.Confirmed,
                draftApprenticeshipCreatedEvent.CreatedOn,
                draftApprenticeshipCreatedEvent.CohortId,
                draftApprenticeshipCreatedEvent.DraftApprenticeshipId);
        }
    }
}
