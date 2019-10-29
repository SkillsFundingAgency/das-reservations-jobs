using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public bool IsLevyAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public short Status { get; set; }
        public string CourseId { get; set; }
        public virtual Course Course{ get; set; }
        public long AccountLegalEntityId { get; set; }
        public uint? ProviderId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public Guid? UserId { get; set; }

        public Reservation Clone()
        {
            return new Reservation
            {
                Id = Id,
                AccountId = AccountId,
                IsLevyAccount = IsLevyAccount,
                CreatedDate = CreatedDate,
                StartDate = StartDate,
                ExpiryDate = ExpiryDate,
                Status = Status,
                CourseId = CourseId,
                Course = Course,
                AccountLegalEntityId = AccountLegalEntityId,
                ProviderId = ProviderId,
                AccountLegalEntityName = AccountLegalEntityName,
                TransferSenderAccountId = TransferSenderAccountId,
                UserId = UserId
            };
        }
    }
}
