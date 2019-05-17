using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface IAccountLegalEntityRepository
    {
        Task Add(AccountLegalEntity accountLegalEntity);
        Task UpdateAgreementStatus(AccountLegalEntity accountLegalEntity);
        Task Remove(AccountLegalEntity accountLegalEntity);
    }
}
