using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public interface IApprenticeshipCourseService
    {
        List<Course> GetCourseInformation();
    }
}
