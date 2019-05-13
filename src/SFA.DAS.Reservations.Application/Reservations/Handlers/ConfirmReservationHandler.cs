using System;
using System.Threading.Tasks;
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

        public async Task Handle(Guid reservationId)
        {
            if (reservationId == null || reservationId.Equals(Guid.Empty))
            {
                throw new ArgumentException("ReservationId must be set", nameof(reservationId));
            }

            await _service.UpdateReservationStatus(reservationId, ReservationStatus.Confirmed);
        }
    }
}
