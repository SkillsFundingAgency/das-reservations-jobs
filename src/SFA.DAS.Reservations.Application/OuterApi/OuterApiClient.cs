using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Application.OuterApi.Requests;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Application.OuterApi;

public interface IOuterApiClient
{
    Task<TResponse> Get<TResponse>(IGetApiRequest request);
}

public class OuterApiClient(HttpClient httpClient, ReservationsJobs configuration) : IOuterApiClient
{
    public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
    {
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
        AddAuthenticationHeader(httpRequestMessage);

        using var response = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        return JsonConvert.DeserializeObject<TResponse>(json);
    }
    
    private void AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
    {
        httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", configuration.ReservationsApimSubscriptionKey);
        httpRequestMessage.Headers.Add("X-Version", "1");
    }
}