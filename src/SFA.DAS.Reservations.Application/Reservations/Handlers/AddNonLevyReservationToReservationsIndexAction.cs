using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class AddNonLevyReservationToReservationsIndexAction : IAddNonLevyReservationToReservationsIndexAction
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<AddNonLevyReservationToReservationsIndexAction> _logger;

        public AddNonLevyReservationToReservationsIndexAction(
            IReservationService reservationService,
            ILogger<AddNonLevyReservationToReservationsIndexAction> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        public async Task Execute(Reservation reservation)
        {
            if (EventIsFromLevyAccount(reservation))
            {
                _logger.LogInformation($"Reservation [{reservation.Id}] is from levy account, no further processing.");
                return;
            }

            await _reservationService.AddReservationToReservationsIndex(reservation);
        }

        private bool EventIsFromLevyAccount(Reservation reservation)
        {
            return reservation.CourseId == null && reservation.StartDate == DateTime.MinValue;
        }
    }
}