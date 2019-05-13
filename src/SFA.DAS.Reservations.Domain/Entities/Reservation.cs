using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public short Status { get; set; }
    }
}
