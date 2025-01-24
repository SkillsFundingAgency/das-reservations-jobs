using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public class GetCourseInformation
{
    private readonly ILogger<GetCourseInformation> _logger;
    private readonly IGetCoursesHandler _handler;

    public GetCourseInformation(ILogger<GetCourseInformation> logger, IGetCoursesHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }

    [Function("GetCourseInformation")]
    [QueueOutput(QueueNames.StoreCourse)]
    public IEnumerable<Course> Run(
        [QueueTrigger(QueueNames.GetCourses, Connection = "AzureWebJobsStorage")] string message)
    {
        var courses = _handler.Handle();
        _logger.LogTrace(
            $"C# Queue trigger function processed message: {message} - adding {courses.Count} courses");
        return courses;
    }
}