
using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.ReservationIndex
{
    public static class IndexRefresh
    {
        [FunctionName("IndexRefresh")]
        public static async Task Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo timerInfo, [Inject]ILogger<ReservationIndexRefreshHandler> log, [Inject]IReservationIndexRefreshHandler handler)
        {
            log.LogInformation($"Running reservation index refresh at: {DateTime.Now}");

            await handler.Handle();

            log.LogInformation($"Finished  reservation index refresh at: {DateTime.Now}");
        }
    }
}
