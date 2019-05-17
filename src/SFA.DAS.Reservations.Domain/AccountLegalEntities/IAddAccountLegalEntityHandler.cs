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
    public interface ISignedLegalAgreementHandler
    {
        Task Handle(SignedAgreementEvent accountLegalEntityAddedEvent);
    }
    public interface IRemoveLegalEntityHandler
    {
        Task Handle(AccountLegalEntityRemovedEvent accountLegalEntityAddedEvent);
    }
}
