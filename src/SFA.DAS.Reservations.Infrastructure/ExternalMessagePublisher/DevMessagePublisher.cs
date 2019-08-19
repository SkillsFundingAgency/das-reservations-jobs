using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.ExternalMessagePublisher
{
    public class DevMessagePublisher : IExternalMessagePublisher
    {

        public async Task SendMessage(string message)
        {
            
        }
    }
}