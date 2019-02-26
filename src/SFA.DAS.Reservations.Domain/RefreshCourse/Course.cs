namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public class Course
    {
        public Course(string id, string title, int level)
        {
            Id = id;
            Title = title;
            Level = level;
        }
        public string Id { get; }
        public string Title { get; }
        public int Level { get; }
    }
}
