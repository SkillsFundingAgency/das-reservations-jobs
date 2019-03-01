using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Handlers
{
    public class StoreCourseHandler : IStoreCourseHandler
    {
        private readonly ICourseService _courseService;

        public StoreCourseHandler(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task Handle(Course course)
        {
            await _courseService.Store(course);
        }
    }
}
