using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.RefreshCourse;

public static class StoreCourse
{
    [Function("StoreCourse")]
    public static async Task Run(
        [QueueTrigger(QueueNames.StoreCourse, Connection = "AzureWebJobsStorage")] Course course,
        FunctionContext context)
    {
        var logger = context.GetLogger("StoreCourse");
        var handler = context.InstanceServices.GetService<IStoreCourseHandler>();

        logger.LogInformation($"C# Queue trigger function processed: {JsonConvert.SerializeObject(course)}");
        await handler.Handle(course);
    }
}