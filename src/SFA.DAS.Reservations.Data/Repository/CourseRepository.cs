using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using Course = SFA.DAS.Reservations.Domain.Entities.Course;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public CourseRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }

        public async Task Add(Course course)
        {
            using (var transaction = _reservationsDataContext.Database.BeginTransaction())
            {
                var courseStored = await _reservationsDataContext.Apprenticeships.FindAsync(course.CourseId);

                if (courseStored != null)
                {
                    courseStored.Level = course.Level;
                    courseStored.Title = course.Title;
                    courseStored.EffectiveTo = course.EffectiveTo;
                }
                else
                {
                    await _reservationsDataContext.Apprenticeships.AddAsync(course);
                }
                _reservationsDataContext.SaveChanges();
                transaction.Commit();
            }
        }
    }
}
