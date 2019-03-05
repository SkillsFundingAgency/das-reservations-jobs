using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public interface ICourseRepository
    {
        Task Add(Entities.Course course);
    }
}
