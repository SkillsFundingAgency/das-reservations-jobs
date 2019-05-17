using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface IAddAccountLegalEntityHandler
    {
        Task Handle(AccountLegalEntityAddedEvent accountLegalEntityAddedEvent);
    }
}
