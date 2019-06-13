using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.RefreshCourse;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.RefreshCourse
{
    public static class GetCourseInformation
    {
        [FunctionName("GetCourseInformation")]
        
        public static void Run([QueueTrigger(QueueNames.GetCourses)]string message, [Inject]ILogger<string> log, [Inject]IGetCoursesHandler handler, [Queue(QueueNames.StoreCourse)]ICollector<Course> outputQueue)
        {
            var courses = handler.Handle();

            log.LogTrace($"C# Queue trigger function processed message: {message} - adding {courses.Count} courses");

            foreach (var course in courses)
            {
                outputQueue.Add(course);
            }
        }
    }
    
}
