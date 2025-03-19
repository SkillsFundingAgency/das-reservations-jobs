using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public class RefreshCourseHttp(ILogger<RefreshCourseHttp> logger)
{
    [Function("RefreshCourseHttp")]
    [QueueOutput(QueueNames.GetCourses)]
    public string Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        logger.LogInformation("C# RefreshCourseHttp trigger function processed a request.");

        return "store";
    }
}