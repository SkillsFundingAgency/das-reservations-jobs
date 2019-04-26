using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public enum ReservationStatus
    {
        Pending = 0,
        Confirmed = 1,
        Completed = 2,
        Deleted = 3
    }

    public class Reservation
    {
        public Guid Id { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
