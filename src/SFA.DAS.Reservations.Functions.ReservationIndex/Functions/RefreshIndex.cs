using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.Functions;

public class RefreshIndex
{
    private readonly ILogger<ReservationIndexRefreshHandler> _logger;
    private readonly IReservationIndexRefreshHandler _handler;

    public RefreshIndex(ILogger<ReservationIndexRefreshHandler> logger, IReservationIndexRefreshHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }
    [Function("RefreshIndex")]
    public async Task Run([QueueTrigger(QueueNames.RefreshReservationIndex)] string message)
    {
        _logger.LogInformation($"Running reservation index refresh at: {DateTime.Now}");

        _handler.Handle();

        _logger.LogInformation($"Finished  reservation index refresh at: {DateTime.Now}");
    }
}