using System;

namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public class Course
    {
        // Required for deserialization from queue.
        public Course() { }

        public Course(int id, string title, int level, DateTime? effectiveTo, string? apprenticeshipType = null)
        {
            Id = id.ToString();
            Title = title;
            Level = level;
            EffectiveTo = effectiveTo;
            ApprenticeshipType = apprenticeshipType;
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? ApprenticeshipType { get; set; }
    }
}
