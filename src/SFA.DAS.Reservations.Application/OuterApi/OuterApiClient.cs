using System;
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

public class OuterApiClient: IOuterApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ReservationsJobs _configuration;

    public OuterApiClient(HttpClient httpClient, ReservationsJobs configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        var baseAddress = configuration.ReservationsApimUrl.EndsWith('/') ? configuration.ReservationsApimUrl : $"{configuration.ReservationsApimUrl}/";
        _httpClient.BaseAddress = new Uri(baseAddress);
    }
    public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
    {
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
        AddAuthenticationHeader(httpRequestMessage);

        using var response = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        return JsonConvert.DeserializeObject<TResponse>(json);
    }
    
    private void AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
    {
        httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _configuration.ReservationsApimSubscriptionKey);
        httpRequestMessage.Headers.Add("X-Version", "1");
    }
}