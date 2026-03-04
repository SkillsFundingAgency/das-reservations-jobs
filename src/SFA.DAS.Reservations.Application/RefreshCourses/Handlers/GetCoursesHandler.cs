using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Handlers;

public class GetCoursesHandler(ICourseImportService courseImportService) : IGetCoursesHandler
{
    public IList<Course> Handle()
    {
        return courseImportService.GetCourseInformation();
    }
}