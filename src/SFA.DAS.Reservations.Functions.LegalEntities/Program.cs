using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Infrastructure.Database;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Validators;
using SFA.DAS.Reservations.Application.Accounts.Handlers;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Functions.LegalEntities;
using SFA.DAS.Reservations.Infrastructure.AzureServiceBus;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(builder => builder.BuildDasConfiguration())
    .ConfigureNServiceBus()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));

        services.AddOptions();

        services.Configure<ReservationsJobs>(configuration.GetSection("ReservationsJobs"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

        var config = configuration.GetSection("ReservationsJobs").Get<ReservationsJobs>();
        services.AddDasLogging();

        services.AddDatabaseRegistration(config, configuration["EnvironmentName"]);

        services.AddTransient<IAzureQueueService, AzureQueueService>();
        services.AddTransient<IAccountLegalEntitiesService, AccountLegalEntitiesService>();
        services.AddTransient<IAccountsService, AccountsService>();

        services.AddHttpClient<IOuterApiClient, OuterApiClient>();

        services.AddTransient<IAccountLegalEntityRepository, AccountLegalEntityRepository>();
        services.AddTransient<IAccountRepository, AccountRepository>();

        services.AddTransient<IAddAccountLegalEntityHandler, AddAccountLegalEntityHandler>();
        services.AddTransient<IRemoveLegalEntityHandler, RemoveLegalEntityHandler>();
        services.AddTransient<ISignedLegalAgreementHandler, SignedLegalAgreementHandler>();
        services.AddTransient<ILevyAddedToAccountHandler, LevyAddedToAccountHandler>();
        services.AddTransient<IAddAccountHandler, AddAccountHandler>();
        services.AddTransient<IAccountNameUpdatedHandler, AccountNameUpdatedHandler>();

        services.AddSingleton<IValidator<AddedLegalEntityEvent>, AddAccountLegalEntityValidator>();
    })
    .Build();

host.Run();