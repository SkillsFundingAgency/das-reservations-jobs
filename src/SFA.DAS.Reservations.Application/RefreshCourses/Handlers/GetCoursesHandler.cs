using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Handlers
{
    public class GetCoursesHandler(IApprenticeshipCourseService apprenticeshipCourseService) : IGetCoursesHandler
    {
        public IList<Course> Handle()
        {
            return apprenticeshipCourseService.GetCourseInformation();
        }
    }
}
