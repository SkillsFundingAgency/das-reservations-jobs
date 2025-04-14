using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public class GetCourseInformation(ILogger<GetCourseInformation> logger, IGetCoursesHandler handler)
{
    [Function("GetCourseInformation")]
    [QueueOutput(QueueNames.StoreCourse)]
    public IEnumerable<Course> Run(
        [QueueTrigger(QueueNames.GetCourses, Connection = "AzureWebJobsStorage")] string message)
    {
        var courses = handler.Handle();
        logger.LogTrace(
            $"C# Queue trigger function processed message: {message} - adding {courses.Count} courses");
        return courses;
    }
}