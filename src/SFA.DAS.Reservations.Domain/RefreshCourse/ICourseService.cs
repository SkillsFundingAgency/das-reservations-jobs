using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public interface ICourseService
    {
        Task Store(Course course);
    }
}