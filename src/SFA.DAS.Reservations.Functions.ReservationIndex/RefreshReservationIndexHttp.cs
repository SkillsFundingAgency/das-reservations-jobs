using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.ReservationIndex
{
    public static class RefreshReservationIndexHttp
    {
        [FunctionName("IndexRefresh")]
        [return: Queue(QueueNames.RefreshReservationIndex)]
        public static string Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, [Inject]ILogger<ReservationIndexRefreshHandler> log, [Inject]IReservationIndexRefreshHandler handler)
        {
            log.LogInformation("C# RefreshIndexHttp trigger function processed a request.");

            return "refresh";
        }
    }
}
