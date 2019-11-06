using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class UpdateReservationIndexAction : IUpdateReservationIndexAction
    {
        private readonly IReservationService _reservationService;

        public UpdateReservationIndexAction(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public async Task Execute(Reservation reservation)
        {
            await _reservationService.AddReservationToReservationsIndex(reservation);
        }
    }
}