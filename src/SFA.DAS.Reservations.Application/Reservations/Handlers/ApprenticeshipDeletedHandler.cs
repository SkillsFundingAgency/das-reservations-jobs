using System;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Messages.Events;
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
        public async Task Handle(DraftApprenticeshipDeletedEvent apprenticeshipDeletedEvent)
        {
            if (!apprenticeshipDeletedEvent.ReservationId.HasValue)
                throw new ArgumentException($"ReservationId is missing from DraftApprenticeshipDeletedEvent, DraftApprenticeshipId:[{apprenticeshipDeletedEvent.DraftApprenticeshipId}].");

            await _reservationService.UpdateReservationStatus(
                apprenticeshipDeletedEvent.ReservationId.Value, 
                ReservationStatus.Pending);
           
        }
    }
}