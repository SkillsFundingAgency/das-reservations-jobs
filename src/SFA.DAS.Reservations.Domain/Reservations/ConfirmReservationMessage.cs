using System;
using NServiceBus;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ConfirmReservationMessage : IMessage 
    {
        public Guid ReservationId { get; set; }
    }
}
