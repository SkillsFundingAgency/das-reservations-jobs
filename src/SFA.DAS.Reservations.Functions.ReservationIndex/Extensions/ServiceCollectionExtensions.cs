using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Application.Reservations.Handlers;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Functions.ReservationIndex.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IReservationIndexRefreshHandler,ReservationIndexRefreshHandler>();
        services.AddTransient<IReservationService,ReservationService>();
        services.AddTransient<IReservationRepository,ReservationRepository>();
        services.AddTransient<IReservationIndexRepository,ReservationIndexRepository>();
        services.AddTransient<IProviderPermissionRepository,ProviderPermissionRepository>();
        services.AddTransient<IIndexRegistry,IndexRegistry>();
        
        return services;
    }
}