using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ApprenticeshipDeletedHandler : IApprenticeshipDeletedHandler
    {
        private readonly IReservationService _reservationService;

        public ApprenticeshipDeletedHandler(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }
        public async Task Handle(Guid reservationId)
        {
            var reservation = await _reservationService.GetReservation(reservationId).ConfigureAwait(false);

            if(reservation is null)
                return;

            var reservationStatusToSet = reservation.Status == ReservationStatus.Change
                ? ReservationStatus.Deleted
                : ReservationStatus.Pending;

            await _reservationService.UpdateReservationStatus(reservationId, reservationStatusToSet);
        }
    }
}