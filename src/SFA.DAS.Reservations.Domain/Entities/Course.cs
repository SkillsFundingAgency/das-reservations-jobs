using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Course
    {
        public string CourseId { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? ApprenticeshipType { get; set; }
    }
}
