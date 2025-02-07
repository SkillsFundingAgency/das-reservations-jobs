using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.Functions;

public class RefreshIndex(ILogger<ReservationIndexRefreshHandler> logger, IReservationIndexRefreshHandler handler)
{
    [Function("RefreshIndex")]
    public async Task Run([QueueTrigger(QueueNames.RefreshReservationIndex)] string message)
    {
        logger.LogInformation($"Running reservation index refresh at: {DateTime.Now}");

        handler.Handle();

        logger.LogInformation($"Finished  reservation index refresh at: {DateTime.Now}");
    }
}