using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.ImportTypes;

namespace SFA.DAS.Reservations.Infrastructure.Api
{
    public class FindApprenticeshipTrainingService
    {
        private readonly HttpClient _client;
        private readonly ReservationsJobs _configuration;

        public FindApprenticeshipTrainingService(HttpClient client, IOptions<ReservationsJobs> configuration)
        {
            _client = client;
            _configuration = configuration.Value;
        }

        public async Task<List<Standard>> GetStandards()
        {
            AddHeaders();

            var response = await _client.GetAsync(_configuration.FindApprenticeshipTrainingOuterApiUrl).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<Standard>>(json);
        }
        
        private void AddHeaders()
        {
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration.FindApprenticeshipTrainingOuterApiKey);
            _client.DefaultRequestHeaders.Add("X-Version", "1");
        }
    }
}