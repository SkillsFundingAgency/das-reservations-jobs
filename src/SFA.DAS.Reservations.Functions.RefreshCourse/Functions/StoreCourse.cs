using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public class StoreCourse(IStoreCourseHandler handler, ILogger<StoreCourse> logger)
{
    [Function("StoreCourse")]
    public async Task Run(
        [QueueTrigger(QueueNames.StoreCourse, Connection = "AzureWebJobsStorage")] Course course,
        FunctionContext context)
    {
        logger.LogInformation($"C# Queue trigger function processed: {JsonConvert.SerializeObject(course)}");
        await handler.Handle(course);
    }
}