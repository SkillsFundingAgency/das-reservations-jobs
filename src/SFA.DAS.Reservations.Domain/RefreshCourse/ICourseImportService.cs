using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.RefreshCourse;

public interface ICourseImportService
{
    List<Course> GetCourseInformation();
}