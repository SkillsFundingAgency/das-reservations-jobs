﻿using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Validators;
using SFA.DAS.Reservations.Application.Accounts.Handlers;
using SFA.DAS.Reservations.Application.Accounts.Services;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Accounts;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Functions.LegalEntities;
using SFA.DAS.Reservations.Infrastructure.AzureServiceBus;
using SFA.DAS.Reservations.Infrastructure.DependencyInjection;
using SFA.DAS.Reservations.Infrastructure.Logging;

[assembly: WebJobsStartup(typeof(Startup))]
namespace SFA.DAS.Reservations.Functions.LegalEntities
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExecutionContextBinding();
            builder.AddDependencyInjection<ServiceProviderBuilder>();
            builder.AddExtension<NServiceBusExtensionConfig>();
        }
    }
    public class ServiceProviderBuilder : IServiceProviderBuilder
    {
        public ServiceCollection ServiceCollection { get; set; }

        private readonly ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }
        public ServiceProviderBuilder(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _loggerFactory = loggerFactory;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables();
            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(',');
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                });
            }
            

            Configuration = config.Build();
        }

        public IServiceProvider Build()
        {
            var services = ServiceCollection ?? new ServiceCollection();

            services.Configure<ReservationsJobs>(Configuration.GetSection("ReservationsJobs"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ReservationsJobs>>().Value);

            services.Configure<AccountApiConfiguration>(Configuration.GetSection("AccountApiConfiguration"));
            services.AddSingleton<IAccountApiConfiguration>(cfg =>  cfg.GetService<IOptions<AccountApiConfiguration>>().Value);
            
            var serviceProvider = services.BuildServiceProvider();

            var config = serviceProvider.GetService<ReservationsJobs>();

            var nLogConfiguration = new NLogConfiguration();

            if (!Configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddLogging((options) =>
                {
                    options.SetMinimumLevel(LogLevel.Trace);
                    options.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                    options.AddConsole();
                    options.AddDebug();

                    nLogConfiguration.ConfigureNLog(Configuration);
                });
            }

            if (Configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ReservationsDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.Reservations")
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            }
            else
            {
                services.AddDbContext<ReservationsDataContext>(options =>
                    options.UseSqlServer(config.ConnectionString));
            }
            services.AddScoped<IReservationsDataContext, ReservationsDataContext>(provider => provider.GetService<ReservationsDataContext>());

            services.AddTransient<IAzureQueueService, AzureQueueService>();
            services.AddTransient<IAccountLegalEntitiesService, AccountLegalEntitiesService>();
            services.AddTransient<IAccountsService, AccountsService>();
            services.AddTransient<IAccountApiClient, AccountApiClient>();
            
            services.AddTransient<IAccountLegalEntityRepository, AccountLegalEntityRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();

            services.AddTransient<IAddAccountLegalEntityHandler, AddAccountLegalEntityHandler>();
            services.AddTransient<IRemoveLegalEntityHandler, RemoveLegalEntityHandler>();
            services.AddTransient<ISignedLegalAgreementHandler, SignedLegalAgreementHandler>();
            services.AddTransient<ILevyAddedToAccountHandler, LevyAddedToAccountHandler>();
            services.AddTransient<IAddAccountHandler, AddAccountHandler>();
            services.AddTransient<IAccountNameUpdatedHandler, AccountNameUpdatedHandler>();

            services.AddSingleton<IValidator<AddedLegalEntityEvent>, AddAccountLegalEntityValidator>();

            //services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            return services.BuildServiceProvider();
        }
    }
}
