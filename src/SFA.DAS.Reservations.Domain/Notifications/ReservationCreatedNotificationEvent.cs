using System;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Domain.Notifications
{
    public class ReservationCreatedNotificationEvent : INotificationEvent
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseLevel { get; set; }
        public uint? ProviderId { get; set; }
        public bool EmployerDeleted { get; set; }

        public static implicit operator ReservationCreatedNotificationEvent(ReservationCreatedEvent source)
        {
            return new ReservationCreatedNotificationEvent            
            {
                Id = source.Id,
                AccountId = source.AccountId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                AccountLegalEntityName = source.AccountLegalEntityName,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                CreatedDate = source.CreatedDate,
                CourseId = source.CourseId,
                CourseName = source.CourseName,
                CourseLevel = source.CourseLevel,
                ProviderId = source.ProviderId,
                EmployerDeleted = false
            };
        }
    }
}