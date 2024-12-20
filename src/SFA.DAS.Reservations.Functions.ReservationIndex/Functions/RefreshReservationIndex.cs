﻿using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.Functions;

public class RefreshReservationIndex
{
    [FunctionName("RefreshReservationIndex")]
    [return: Queue(QueueNames.RefreshReservationIndex)]
    public static string Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, [Inject]ILogger<string> log)
    {
        log.LogInformation("Timer trigger function for reservation index refresh executed");
            
        return "refresh";
    }
}