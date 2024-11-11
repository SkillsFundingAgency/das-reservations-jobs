using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.RefreshCourse.Functions;

public static class StoreCourse
{
    [FunctionName("StoreCourse")]
    public static async Task Run([QueueTrigger(QueueNames.StoreCourse)]Course course, [Inject]ILogger<Course> log, [Inject]IStoreCourseHandler handler)
    {
        log.LogInformation("StoreCourse Function processed: {Data}", JsonConvert.SerializeObject(course));
        
        await handler.Handle(course);
    }
}