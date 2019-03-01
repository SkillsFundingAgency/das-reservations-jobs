using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.RefreshCourse
{
    public static class StoreCourse
    {
        [FunctionName("StoreCourse")]
        public static async Task Run([QueueTrigger(QueueNames.StoreCourse)]Course course, ILogger log, [Inject]IStoreCourseHandler handler)
        {
            log.LogInformation($"C# Queue trigger function processed: {JsonConvert.SerializeObject(course)}");
            await handler.Handle(course);
        }
    }
}
