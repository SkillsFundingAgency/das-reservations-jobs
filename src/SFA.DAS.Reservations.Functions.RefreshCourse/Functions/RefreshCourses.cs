using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public class RefreshCourses
{
    private readonly ILogger<RefreshCourses> _logger;

    public RefreshCourses(ILogger<RefreshCourses> logger)
    {
        _logger = logger;
    }

    [Function("RefreshCourses")]
    [QueueOutput("get-courses")]
    public string Run([TimerTrigger("0 0 0 */1 * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        return "get-courses";
    }
}