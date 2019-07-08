using System;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Messages.Events;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IApprenticeshipDeletedHandler
    {
        Task Handle(Guid reservationId);
    }
}