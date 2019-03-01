using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class CourseRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public CourseRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }

        public async Task Add(Course course)
        {
            await _reservationsDataContext.Apprenticeships.AddAsync(course);

            _reservationsDataContext.SaveChanges();
        }
    }
}
