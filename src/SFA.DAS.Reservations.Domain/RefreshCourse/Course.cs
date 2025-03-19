using System;

namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public class Course(int id, string title, int level, DateTime? effectiveTo)
    {
        public string Id { get; } = id.ToString();
        public string Title { get; } = title;
        public int Level { get; } = level;
        public DateTime? EffectiveTo { get; } = effectiveTo;
    }
}
