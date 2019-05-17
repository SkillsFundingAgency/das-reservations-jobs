using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Services
{
    public class AccountLegalEntitiesService
    {
        private readonly IAccountLegalEntityRepository _repository;

        public AccountLegalEntitiesService(IAccountLegalEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAccountLegalEntity(AccountLegalEntityAddedEvent accountLegalEntity)
        {
            await _repository.Add(MapAccountLegalEntity(accountLegalEntity));
        }

        public async Task SignAgreementForAccountLegalEntity(SignedAgreementEvent signedAgreementEvent)
        {
            await _repository.UpdateAgreementStatus(MapAccountLegalEntity(signedAgreementEvent));
        }

        public async Task RemoveAccountLegalEntity(AccountLegalEntityRemovedEvent accountLegalEntityRemovedEvent)
        {
            await _repository.Remove(MapAccountLegalEntity(accountLegalEntityRemovedEvent));
        }

        private AccountLegalEntity MapAccountLegalEntity(SignedAgreementEvent signedAgreementEvent)
        {
            return new AccountLegalEntity
            {
                AccountId = signedAgreementEvent.AccountId,
                LegalEntityId = signedAgreementEvent.LegalEntityId
            };
        }
        private AccountLegalEntity MapAccountLegalEntity(AccountLegalEntityRemovedEvent accountLegalEntityRemovedEvent)
        {
            return new AccountLegalEntity
            {
                AccountLegalEntityId = accountLegalEntityRemovedEvent.AccountLegalEntityId
            };
        }


        private AccountLegalEntity MapAccountLegalEntity(AccountLegalEntityAddedEvent accountLegalEntity)
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
