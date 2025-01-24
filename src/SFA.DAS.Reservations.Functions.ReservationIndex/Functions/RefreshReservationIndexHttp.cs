using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.Functions;

public class RefreshReservationIndexHttp
{
    private readonly ILogger<ReservationIndexRefreshHandler> _logger;
    private readonly IReservationIndexRefreshHandler _handler;

    public RefreshReservationIndexHttp(ILogger<ReservationIndexRefreshHandler> logger, IReservationIndexRefreshHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }

    [Function("IndexRefresh")]
    [QueueOutput(QueueNames.RefreshReservationIndex)]
    public string Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("C# RefreshIndexHttp trigger function processed a request.");

        return "refresh";
    }
}