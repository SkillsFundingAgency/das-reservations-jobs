using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public class RefreshCourses(ILogger<RefreshCourses> logger)
{
    [Function("RefreshCourses")]
    [QueueOutput(QueueNames.GetCourses)]
    public string Run([TimerTrigger("0 0 0 */1 * *")] TimerInfo myTimer)
    {
        logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        return "get-courses";
    }
}