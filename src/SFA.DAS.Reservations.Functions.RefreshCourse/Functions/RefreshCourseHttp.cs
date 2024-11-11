using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public static class RefreshCourseHttp
{
    [FunctionName("RefreshCourseHttp")]
    [return: Queue(QueueNames.GetCourses)]
    public static string Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, [Inject]ILogger<string> log)
    {
        log.LogInformation("RefreshCourseHttp trigger function processed a request.");

        return "store";
    }
}