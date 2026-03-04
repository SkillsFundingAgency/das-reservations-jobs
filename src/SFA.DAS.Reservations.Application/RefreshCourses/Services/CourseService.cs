using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Extensions;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Domain.Types;

namespace SFA.DAS.Reservations.Application.RefreshCourses.Services;

public class CourseService(ICourseRepository courseRepository) : ICourseService
{
    public async Task Store(Course course)
    {
        await courseRepository.Add(MapCourse(course));
    }

    private Domain.Entities.Course MapCourse(Course course)
    {
        return new Domain.Entities.Course
        {
            CourseId = course.Id,
            Title = course.Title,
            Level = course.Level,
            EffectiveTo = course.EffectiveTo == DateTime.MinValue ? null : course.EffectiveTo,
            ApprenticeshipType = course.LearningType,
            LearningType = course.LearningType.ToEnumValue<LearningType>()
        };
    }
}