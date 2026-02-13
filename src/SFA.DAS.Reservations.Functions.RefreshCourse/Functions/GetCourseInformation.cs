using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public class GetCourseInformation(ILogger<GetCourseInformation> logger, IGetCoursesHandler handler, IOptions<ReservationsJobs> options)
{
    private static bool IsShortCourse(Course course)
    {
        var learningType = course.LearningType?.Trim();
        return !string.IsNullOrEmpty(learningType) &&
               string.Equals(learningType, "ApprenticeshipUnit", StringComparison.OrdinalIgnoreCase);
    }

    [Function("GetCourseInformation")]
    [QueueOutput(QueueNames.StoreCourse)]
    public IEnumerable<Course> Run(
        [QueueTrigger(QueueNames.GetCourses, Connection = "AzureWebJobsStorage")] string message)
    {
        var courses = handler.Handle();
        var includeShortCourses = options.Value.IncludeShortCourses;
        var toQueue = includeShortCourses
            ? courses
            : courses.Where(c => !IsShortCourse(c)).ToList();
        if (!includeShortCourses && courses.Count != toQueue.Count)
        {
            logger.LogInformation("IncludeShortCourses is disabled - excluding {Excluded} short courses from queue",
                courses.Count - toQueue.Count);
        }

        logger.LogTrace("C# Queue trigger function processed message: {Message} - adding {Count} courses to queue", message, toQueue.Count);
        
        return toQueue;
    }
}