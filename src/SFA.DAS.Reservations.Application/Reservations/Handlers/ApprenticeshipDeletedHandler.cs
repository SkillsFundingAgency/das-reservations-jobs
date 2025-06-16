using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class ApprenticeshipDeletedHandler(IReservationService reservationService) : IApprenticeshipDeletedHandler
    {
        public async Task Handle(Guid reservationId)
        {
            var reservation = await reservationService.GetReservation(reservationId).ConfigureAwait(false);

            if(reservation is null)
                return;

            var reservationStatusToSet = reservation.Status == ReservationStatus.Change
                ? ReservationStatus.Deleted
                : ReservationStatus.Pending;

            await reservationService.UpdateReservationStatus(reservationId, reservationStatusToSet);
        }
    }
}