using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public interface IGetCoursesHandler
    {
        IList<Course> Handle();
    }
}
