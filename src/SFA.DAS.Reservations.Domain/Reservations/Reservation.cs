using System;
using SFA.DAS.Reservations.Messages;

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
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public int CourseLevel { get; set; }
        public uint? ProviderId { get; set; }
        public bool EmployerDeleted { get; set; }

        public static implicit operator Reservation(ReservationCreatedEvent source)
        {
            return new Reservation            
            {
                Id = source.Id,
                Status = ReservationStatus.Pending,
                AccountId = source.AccountId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                AccountLegalEntityName = source.AccountLegalEntityName,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                CreatedDate = source.CreatedDate,
                CourseId = source.CourseId,
                CourseName = source.CourseName,
                CourseLevel = int.Parse(source.CourseLevel),
                ProviderId = source.ProviderId,
                EmployerDeleted = false
            };
        }
    }
}
