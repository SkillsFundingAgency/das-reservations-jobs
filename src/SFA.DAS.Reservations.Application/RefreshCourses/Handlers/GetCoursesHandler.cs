using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Handlers
{
    public class GetCoursesHandler : IGetCoursesHandler
    {
        private readonly IApprenticeshipCourseService _apprenticeshipCourseService;

        public GetCoursesHandler(IApprenticeshipCourseService apprenticeshipCourseService)
        {
            _apprenticeshipCourseService = apprenticeshipCourseService;
        }

        public IList<Course> Handle()
        {
            return _apprenticeshipCourseService.GetCourseInformation();
        }
    }
}
