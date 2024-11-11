using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Application.RefreshCourses.Handlers;
using SFA.DAS.Reservations.Application.RefreshCourses.Services;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure.Api;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IFindApprenticeshipTrainingService, FindApprenticeshipTrainingService>();
        services.AddTransient<IApprenticeshipCourseService, ApprenticeshipCoursesService>();
        services.AddTransient<ICourseService, CourseService>();
        services.AddTransient<IGetCoursesHandler, GetCoursesHandler>();
        services.AddTransient<IStoreCourseHandler, StoreCourseHandler>();
        services.AddTransient<ICourseRepository, CourseRepository>();
        
        return services;
    }
}