using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.Reservations.Handlers
{
    public class AddNonLevyReservationToReservationsIndexAction(
        IReservationService reservationService,
        ILogger<AddNonLevyReservationToReservationsIndexAction> logger)
        : IAddNonLevyReservationToReservationsIndexAction
    {
        public async Task Execute(Reservation reservation)
        {
            if (EventIsFromLevyAccount(reservation))
            {
                logger.LogInformation($"Reservation [{reservation.Id}] is from levy account, no further processing.");
                return;
            }

            await reservationService.AddReservationToReservationsIndex(reservation);
        }

        private bool EventIsFromLevyAccount(Reservation reservation)
        {
            return reservation.CourseId == null && reservation.StartDate == DateTime.MinValue;
        }
    }
}