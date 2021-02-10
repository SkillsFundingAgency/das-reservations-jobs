using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.ImportTypes;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Infrastructure.Api
{
    public class FindApprenticeshipTrainingService : IFindApprenticeshipTrainingService
    {
        private readonly HttpClient _client;
        private readonly ReservationsJobs _configuration;

        public FindApprenticeshipTrainingService(HttpClient client, IOptions<ReservationsJobs> configuration)
        {
            _client = client;
            _configuration = configuration.Value;
        }

        public async Task<StandardApiResponse> GetStandards()
        {
            AddHeaders();

            var response = await _client.GetAsync(_configuration.ReservationsApimUrl).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<StandardApiResponse>(json);
        }

        public async Task<ProviderApiResponse> GetProvider(uint ukPrn)
        {
            AddHeaders();

            var response = await _client.GetAsync(_configuration.ReservationsApimUrl)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<ProviderApiResponse>(json);
        }
        
        private void AddHeaders()
        {
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration.ReservationsApimSubscriptionKey);
            _client.DefaultRequestHeaders.Add("X-Version", "1");
        }
    }
}