using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.RefreshCourse
{
    public static class RefreshCourses
    {
        [FunctionName("RefreshCourses")]

        public static async Task Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, ILogger log, [Inject]IRefreshCourseHandler refreshCourseHandler)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
        }
    }
}
