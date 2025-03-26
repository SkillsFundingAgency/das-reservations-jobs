using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Services;

public class ApprenticeshipCoursesService(IFindApprenticeshipTrainingService standardApiClient)
    : IApprenticeshipCourseService
{
    public List<Course> GetCourseInformation()
    {
        var list = new ConcurrentBag<Course>();

        var tasks = new List<Task>
        {
            GetStandards(list)
        };

        Task.WaitAll(tasks.ToArray());

        return list.ToList();
    }

    private async Task GetStandards(ConcurrentBag<Course> courses)
    {
        var standardApiResponse = await standardApiClient.GetStandards();

        foreach (var standard in standardApiResponse.Standards)
        {
            courses.Add(new Course(standard.Id, standard.Title, standard.Level, standard.EffectiveTo, standard.ApprenticeshipType));
        }
    }
}