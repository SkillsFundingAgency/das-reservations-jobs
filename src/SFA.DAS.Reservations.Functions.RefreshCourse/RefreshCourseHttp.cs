using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.RefreshCourse
{
    public static class RefreshCourseHttp
    {
        [FunctionName("RefreshCourseHttp")]
        [return: Queue(QueueNames.RefreshCourse)]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return "store";
        }
    }
}
