using System;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ConfirmReservationMessage  
    {
        public Guid ReservationId { get; set; }
        public long CohortId { get; set; }
        public long DraftApprenticeshipId { get; set; }
    }
}
