using System;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IApprenticeshipDeletedHandler
    {
        Task Handle(Guid reservationId);
    }
}