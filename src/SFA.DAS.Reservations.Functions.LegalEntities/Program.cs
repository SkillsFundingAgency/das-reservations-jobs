using Microsoft.Extensions.Hosting;
using SFA.DAS.Reservations.Functions.LegalEntities;
using NServiceBus;
using SFA.DAS.Reservations.Infrastructure.NServiceBus;

[assembly: NServiceBusTriggerFunction(AzureFunctionsQueueNames.LegalEntitiesQueue)]

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(builder => builder.BuildDasConfiguration())
    .ConfigureNServiceBus(AzureFunctionsQueueNames.LegalEntitiesQueue)
    .ConfigureServices((context, services) =>
    {
        var servicesRegistration = new ServicesRegistration(services, context.Configuration);
        servicesRegistration.Register();
    })
    .Build();

host.Run();
