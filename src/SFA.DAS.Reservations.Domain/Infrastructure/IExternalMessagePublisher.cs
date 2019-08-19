using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public interface IExternalMessagePublisher
    {
        Task SendMessage(string message);
    }
}
