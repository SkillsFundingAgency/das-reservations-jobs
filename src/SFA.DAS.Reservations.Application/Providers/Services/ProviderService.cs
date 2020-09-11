using System.Threading.Tasks;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Application.Providers.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IFindApprenticeshipTrainingService _providerApiClient;

        public ProviderService(IFindApprenticeshipTrainingService providerApiClient)
        {
            _providerApiClient = providerApiClient;
        }

        public async Task<ProviderDetails> GetDetails(uint providerId)
        {
            ProviderDetails providerDetails = await _providerApiClient.GetProvider(providerId);
            return providerDetails;
        }
    }
}