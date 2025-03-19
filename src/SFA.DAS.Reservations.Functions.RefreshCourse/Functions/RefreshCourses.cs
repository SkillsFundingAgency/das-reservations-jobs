using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public class RefreshCourses(ILogger<RefreshCourses> logger)
{
    [Function("RefreshCourses")]
    [QueueOutput("get-courses")]
    public string Run([TimerTrigger("0 0 0 */1 * *")] TimerInfo myTimer)
    {
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        return "get-courses";
    }
}