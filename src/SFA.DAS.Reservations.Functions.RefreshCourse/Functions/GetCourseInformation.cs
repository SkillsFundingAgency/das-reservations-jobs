using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public class GetCourseInformation(ILogger<GetCourseInformation> logger, IGetCoursesHandler handler, IOptions<ReservationsJobs> options)
{
    [Function("GetCourseInformation")]
    [QueueOutput(QueueNames.StoreCourse)]
    public IEnumerable<Course> Run(
        [QueueTrigger(QueueNames.GetCourses, Connection = "AzureWebJobsStorage")] string message)
    {
        var courses = handler.Handle();
        var toQueue = courses;

        logger.LogTrace("C# Queue trigger function processed message: {Message} - adding {Count} courses to queue", message, toQueue.Count);
        
        return toQueue;
    }
}