using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.ReservationIndex
{
    public static class IndexRefresh
    {
        [FunctionName("IndexRefresh")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo timerInfo, [Inject] IReservationIndexRefreshHandler handler, ILogger log)
        {
            log.LogInformation($"Running reservation index refresh at: {DateTime.Now}");

            await handler.Handle();

            log.LogInformation($"Finished  reservation index refresh at: {DateTime.Now}");
        }
    }
}
