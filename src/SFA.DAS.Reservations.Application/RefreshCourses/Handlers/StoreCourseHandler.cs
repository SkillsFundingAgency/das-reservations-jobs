using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Handlers
{
    public class StoreCourseHandler(ICourseService courseService) : IStoreCourseHandler
    {
        public async Task Handle(Course course)
        {
            await courseService.Store(course);
        }
    }
}
