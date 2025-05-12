using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Reservations.Application.Services;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Domain.Interfaces;

namespace SFA.DAS.Reservations.Infrastructure.AzureSearch;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddAzureSearch(this IServiceCollection collection)
    {
        collection.AddTransient<IAzureSearchHelper, AzureSearchHelper>();
        collection.AddTransient<IAzureSearchReservationIndexRepository, AzureSearchReservationIndexRepository>();
        return collection;
    }
} 