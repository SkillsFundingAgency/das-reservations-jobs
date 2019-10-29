using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.ReservationIndex
{
    public class RefreshIndex
    {
        [FunctionName("RefreshIndex")]
        public static async Task Run([QueueTrigger(QueueNames.RefreshReservationIndex)]string message, [Inject]ILogger<ReservationIndexRefreshHandler> log, [Inject]IReservationIndexRefreshHandler handler)
        {
            log.LogInformation($"Running reservation index refresh at: {DateTime.Now}");

            await handler.Handle();

            log.LogInformation($"Finished  reservation index refresh at: {DateTime.Now}");
        }
    }
}
