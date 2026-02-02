using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Services;

public class ApprenticeshipCoursesService(IReferenceDataImportService outerApiClient)
    : IApprenticeshipCourseService
{
    public List<Course> GetCourseInformation()
    {
        var list = new ConcurrentBag<Course>();

        var tasks = new List<Task>
        {
            GetCourses(list)
        };

        Task.WaitAll(tasks.ToArray());

        return list.ToList();
    }

    private async Task GetCourses(ConcurrentBag<Course> courses)
    {
        var courseApiResponse = await outerApiClient.GetCourses();

        foreach (var course in courseApiResponse.Courses)
        {
            courses.Add(new Course(course.Id, course.Title, course.Level, course.EffectiveTo, course.ApprenticeshipType, course.LearningType));
        }
    }
}