using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public static class GetCourseInformation
{
    [FunctionName("GetCourseInformation")]
    public static void Run([QueueTrigger(QueueNames.GetCourses)] string message,
        [Inject] ILogger<string> log,
        [Inject] IGetCoursesHandler handler,
        [Queue(QueueNames.StoreCourse)] ICollector<Course> outputQueue)
    {
        var courses = handler.Handle();

        log.LogTrace("GetCourseInformation Function processed message: {Message} - adding {CoursesCount} courses", message, courses.Count);

        foreach (var course in courses)
        {
            outputQueue.Add(course);
        }
    }
}