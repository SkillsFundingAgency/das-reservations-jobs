using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public short Status { get; set; }
        public long AccountLegalEntityId { get; set; }
        public uint? ProviderId { get; set; }

        public Reservation Clone()
        {
            return new Reservation
            {
                Id = Id,
                AccountId = AccountId,
                AccountLegalEntityId = AccountLegalEntityId,
                ProviderId = ProviderId,
                Status = Status
            };
        }
    }
}
