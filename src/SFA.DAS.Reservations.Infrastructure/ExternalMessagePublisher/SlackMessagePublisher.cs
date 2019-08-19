using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.ExternalMessagePublisher
{
    public class SlackMessagePublisher : IExternalMessagePublisher
    {
        private readonly ReservationsJobs _config;

        public SlackMessagePublisher(IOptions<ReservationsJobs> configOptions)
        {
            _config = configOptions.Value;
        }
        public async Task SendMessage(string message)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.SlackSecret);
                var jsonRequest = JsonConvert.SerializeObject(new { text = message });
                var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_config.SlackChannelUrl, stringContent).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                
            }
        }
    }
}
