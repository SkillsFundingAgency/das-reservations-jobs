using System;

namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public class Course
    {
        public Course(string id, string title, int level, DateTime? effectiveTo)
        {
            Id = id;
            Title = title;
            Level = level;
            EffectiveTo = effectiveTo;
        }
        public string Id { get; }
        public string Title { get; }
        public int Level { get; }
        public DateTime? EffectiveTo { get; }
    }
}
