using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public interface IStoreCourseHandler
    {
        Task Handle();
    }
}