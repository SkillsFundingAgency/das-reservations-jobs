using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.RefreshCourse
{
    public class RefreshCourseHttp
    {
        private readonly ILogger<RefreshCourseHttp> _logger;

        public RefreshCourseHttp(ILogger<RefreshCourseHttp> logger)
        {
            _logger = logger;
        }

        [Function("RefreshCourseHttp")]
        [QueueOutput(QueueNames.GetCourses)]
        public string Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# RefreshCourseHttp trigger function processed a request.");

            return "store";
        }
    }
}
