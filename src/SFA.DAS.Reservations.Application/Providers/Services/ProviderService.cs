using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.Providers.Services;

public class ProviderService(IFindApprenticeshipTrainingService providerApiClient) : IProviderService
{
    public async Task<ProviderDetails> GetDetails(uint providerId)
    {
        return await providerApiClient.GetProvider(providerId);
    }
}