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
            await _reservationService.UpdateReservationStatus(
                reservationId, 
                ReservationStatus.Pending);
           
        }
    }
}