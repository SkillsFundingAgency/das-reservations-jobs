using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public static class RefreshCourses
{
    [FunctionName("RefreshCourses")]
    [return: Queue(QueueNames.GetCourses)]
    public static string Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, [Inject]ILogger<string> log)
    {
        log.LogInformation("RefreshCourses Function executed");
        
        return "get-courses";
    }
}