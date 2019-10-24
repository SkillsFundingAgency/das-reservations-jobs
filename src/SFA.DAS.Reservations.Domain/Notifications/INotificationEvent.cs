using System;

namespace SFA.DAS.Reservations.Domain.Notifications
{
    public interface INotificationEvent
    {
        Guid Id { get; set; }
        long AccountId { get; set; }
        long AccountLegalEntityId { get; set; }
        string AccountLegalEntityName { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        DateTime CreatedDate { get; set; }
        string CourseId { get; set; }
        string CourseName { get; set; }
        string CourseLevel { get; set; }
        uint? ProviderId { get; set; }
        bool EmployerDeleted { get; set; }
    }
}