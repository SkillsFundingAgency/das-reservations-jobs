using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Services
{
    public class AccountLegalEntitiesService(IAccountLegalEntityRepository repository) : IAccountLegalEntitiesService
    {
        public async Task AddAccountLegalEntity(AddedLegalEntityEvent accountLegalEntity)
        {
            await repository.Add(MapAccountLegalEntity(accountLegalEntity));
        }

        public async Task SignAgreementForAccountLegalEntity(SignedAgreementEvent signedAgreementEvent)
        {
            await repository.UpdateAgreementStatus(MapAccountLegalEntity(signedAgreementEvent));
        }

        public async Task RemoveAccountLegalEntity(RemovedLegalEntityEvent accountLegalEntityRemovedEvent)
        {
            await repository.Remove(MapAccountLegalEntity(accountLegalEntityRemovedEvent));
        }

        private AccountLegalEntity MapAccountLegalEntity(RemovedLegalEntityEvent removedLegalEntityEvent)
        {
            return new AccountLegalEntity
            {
                AccountLegalEntityId = removedLegalEntityEvent.AccountLegalEntityId
            };
        }
        private AccountLegalEntity MapAccountLegalEntity(SignedAgreementEvent signedAgreementEvent)
        {
            return new AccountLegalEntity
            {
                AccountId = signedAgreementEvent.AccountId,
                LegalEntityId = signedAgreementEvent.LegalEntityId
            };
        }


        private AccountLegalEntity MapAccountLegalEntity(AddedLegalEntityEvent accountLegalEntity)
        {
            return new AccountLegalEntity
            {
                Id = Guid.NewGuid(),
                AccountLegalEntityId = accountLegalEntity.AccountLegalEntityId,
                AccountId = accountLegalEntity.AccountId,
                LegalEntityId = accountLegalEntity.LegalEntityId,
                AccountLegalEntityName = accountLegalEntity.OrganisationName
            };
        }
    }
}
