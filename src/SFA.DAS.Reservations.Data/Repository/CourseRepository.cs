using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using Course = SFA.DAS.Reservations.Domain.Entities.Course;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class CourseRepository(IReservationsDataContext reservationsDataContext) : ICourseRepository
    {
        public async Task Add(Course course)
        {
            var courseStored = await reservationsDataContext.Apprenticeships.FindAsync(course.CourseId);

            if (courseStored != null)
            {
                courseStored.Level = course.Level;
                courseStored.Title = course.Title;
                courseStored.EffectiveTo = course.EffectiveTo;
                courseStored.ApprenticeshipType = course.ApprenticeshipType;
            }
            else
            {
                await reservationsDataContext.Apprenticeships.AddAsync(course);
            }
            reservationsDataContext.SaveChanges();

        }
    }
}