using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Application.Accounts.Handlers;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Infrastructure.AzureServiceBus;

namespace SFA.DAS.Reservations.Functions.LegalEntities.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IAzureQueueService, AzureQueueService>();
        services.AddTransient<IAccountLegalEntitiesService, AccountLegalEntitiesService>();
        services.AddTransient<IAccountsService, AccountsService>();
        services.AddTransient<IAccountLegalEntityRepository, AccountLegalEntityRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<IAddAccountLegalEntityHandler, AddAccountLegalEntityHandler>();
        services.AddTransient<IRemoveLegalEntityHandler, RemoveLegalEntityHandler>();
        services.AddTransient<ISignedLegalAgreementHandler, SignedLegalAgreementHandler>();
        services.AddTransient<ILevyAddedToAccountHandler, LevyAddedToAccountHandler>();
        services.AddTransient<IAddAccountHandler, AddAccountHandler>();
        services.AddTransient<IAccountNameUpdatedHandler, AccountNameUpdatedHandler>();
        
        return services;
    }
}