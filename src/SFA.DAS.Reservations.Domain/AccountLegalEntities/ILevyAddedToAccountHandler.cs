using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Messages.Events;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface ILevyAddedToAccountHandler
    {
        Task Handle(LevyAddedToAccount levyAddedToAccountEvent);
    }
}
