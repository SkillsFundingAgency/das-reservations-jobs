using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Services
{
    public class ApprenticeshipCoursesService : IApprenticeshipCourseService
    {
        private readonly IFrameworkApiClient _frameworkApiClient;
        private readonly IStandardApiClient _standardApiClient;

        public ApprenticeshipCoursesService(IFrameworkApiClient frameworkApiClient, IStandardApiClient standardApiClient)
        {
            _frameworkApiClient = frameworkApiClient;
            _standardApiClient = standardApiClient;
        }

        public List<Course> GetCourseInformation()
        {
            var list = new ConcurrentBag<Course>();

            var tasks = new List<Task>
            {
                GetFrameworks(list),
                GetStandards(list)
            };

            Task.WaitAll(tasks.ToArray());

            return list.ToList();
        }

        private async Task GetFrameworks(ConcurrentBag<Course> courses)
        {
            var frameworks = await _frameworkApiClient.GetAllAsync();

            foreach (var framework in frameworks.Where(c=>c.IsActiveFramework))
            {
                courses.Add(new Course(framework.Id,framework.Title,framework.Level));
            }
        }

        private async Task GetStandards(ConcurrentBag<Course> courses)
        {
            var standards = await _standardApiClient.GetAllAsync();

            foreach (var standard in standards.Where(c=>c.IsActiveStandard))
            {
                courses.Add(new Course(standard.Id, standard.Title, standard.Level));
            }
        }
    }
}
