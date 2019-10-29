using System.Threading.Tasks;
using SFA.DAS.Providers.Api.Client;
using SFA.DAS.Reservations.Domain.Providers;

namespace SFA.DAS.Reservations.Application.Providers.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderApiClient _providerApiClient;

        public ProviderService(IProviderApiClient providerApiClient)
        {
            _providerApiClient = providerApiClient;
        }

        public async Task<ProviderDetails> GetDetails(uint providerId)
        {
            ProviderDetails providerDetails = await _providerApiClient.GetAsync(providerId);
            return providerDetails;
        }
    }
}