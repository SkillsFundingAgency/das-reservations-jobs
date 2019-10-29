using System;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ReservationIndex
    {
        public string Id => $@"{ProviderId}_{AccountLegalEntityId}_{ReservationId}";
        
        public Guid ReservationId { get; set; }
        public long AccountId { get; set; }
        public short Status { get; set; }
        public long AccountLegalEntityId { get; set; }
        public uint? ProviderId { get; set; }
    }
}
