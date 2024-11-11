using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.Functions;

public class RefreshReservationIndex
{
    private readonly ILogger<string> _logger;

    public RefreshReservationIndex(ILogger<string> logger)
    {
        _logger = logger;
    }
    [Function("RefreshReservationIndex")]
    [QueueOutput(QueueNames.RefreshReservationIndex)]
    public string Run([TimerTrigger("0 0 0 */1 * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# Timer trigger function for reservation index refresh executed at: {DateTime.Now}");

        return "refresh";
    }
}