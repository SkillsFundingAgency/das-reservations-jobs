using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Application.ProviderPermissions.Handlers;
using SFA.DAS.Reservations.Application.ProviderPermissions.Service;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Infrastructure.AzureServiceBus;

namespace SFA.DAS.Reservations.Functions.ProviderPermission.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IAzureQueueService, AzureQueueService>();
        services.AddTransient<IReservationService, ReservationService>();
        services.AddTransient<IProviderPermissionService, ProviderPermissionService>();
            
        services.AddTransient<IProviderPermissionRepository, ProviderPermissionRepository>();
        services.AddTransient<IReservationRepository, ReservationRepository>();
        services.AddTransient<IReservationIndexRepository, ReservationIndexRepository>();
        services.AddTransient<IIndexRegistry, IndexRegistry>();
        
        services.AddTransient<IProviderPermissionsUpdatedHandler, ProviderPermissionsUpdatedHandler>();
        services.AddTransient<IUpdatedPermissionsEventValidator, UpdatedPermissionsEventValidator>();
        
        return services;
    }
}