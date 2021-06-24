using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }


        public async Task Store(Course course)
        {
            await _courseRepository.Add(MapCourse(course));
        }

        private Domain.Entities.Course MapCourse(Course course)
        {
            return new Domain.Entities.Course
            {
                CourseId = course.Id,
                Title = course.Title,
                Level = course.Level,
                EffectiveTo = course.EffectiveTo == DateTime.MinValue ? null : course.EffectiveTo
            };
        }

    }
}
