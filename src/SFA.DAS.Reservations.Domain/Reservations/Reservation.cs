using System;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public enum ReservationStatus
    {
        Pending = 0,
        Confirmed = 1,
        Completed = 2,
        Deleted = 3,
        Change = 4
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
        public int? CourseLevel { get; set; }
        public uint? ProviderId { get; set; }
        public bool EmployerDeleted { get; set; }

        public Reservation()
        {
        }

        public Reservation(Domain.Entities.Reservation source)
        {

            Id = source.Id;
            Status = (ReservationStatus)source.Status;
            AccountId = source.AccountId;
            AccountLegalEntityId = source.AccountLegalEntityId;
            AccountLegalEntityName = source.AccountLegalEntityName;
            StartDate = source.StartDate.GetValueOrDefault();
            EndDate = source.ExpiryDate.GetValueOrDefault();
            CreatedDate = source.CreatedDate;
            CourseId = source.CourseId;
            CourseName = source.Course?.Title;
            CourseLevel = source.Course?.Level;
            ProviderId = source.ProviderId;

        }

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
                CourseLevel = source.CourseLevel == null ? default : int.Parse(source.CourseLevel),
                ProviderId = source.ProviderId,
                EmployerDeleted = false
            };
        }

       
    }
}
