using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.Reservations.Infrastructure.Attributes;

namespace SFA.DAS.Reservations.Functions.Reservations
{
    public class ProcessOutboxMessages
    {
        [FunctionName("ProcessOutboxMessages")]
        public static async Task Run([TimerTrigger("0 */10 * * * *")] TimerInfo timer, [Inject] IProcessClientOutboxMessagesJob handler)
        {
            await handler.RunAsync();
        }
    }
}