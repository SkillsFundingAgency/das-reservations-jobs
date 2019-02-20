using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Reservations.Jobs.Functions.RefreshCourse
{
    public static class RefreshCourses
    {
        [FunctionName("RefreshCourses")]
        public static void Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
