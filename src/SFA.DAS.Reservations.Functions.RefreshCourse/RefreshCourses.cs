using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.RefreshCourse
{
    public static class RefreshCourses
    {
        [FunctionName("RefreshCourses")]
        [return: Queue(QueueNames.GetCourses)]
        public static string Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            return "get-courses";
        }
    }
}
