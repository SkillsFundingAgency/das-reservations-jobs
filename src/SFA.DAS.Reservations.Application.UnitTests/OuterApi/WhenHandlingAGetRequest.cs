using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.OuterApi.Requests;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Application.UnitTests.OuterApi;

public class WhenHandlingAGetRequest
{
    [Test]
    public async Task Then_The_Endpoint_Is_Called_With_Authentication_Header_And_Data_Returned()
    {
        //Arrange
        const string key = "123-abc-567";
        var getTestRequest = new GetTestRequest();
        var testObject = new List<string>();
        var config = new ReservationsJobs
        {
            ReservationsApimSubscriptionKey = key,
            ReservationsApimUrl = "https://testing.local/"
        };

        var response = new HttpResponseMessage
        {
            Content = new StringContent(JsonConvert.SerializeObject(testObject)),
            StatusCode = HttpStatusCode.Accepted
        };

        var httpMessageHandler = SetupMessageHandlerMock(response, $"{config.ReservationsApimUrl}{getTestRequest.GetUrl}", config.ReservationsApimSubscriptionKey);
        var httClient = new HttpClient(httpMessageHandler.Object)
        {
            BaseAddress = new Uri(config.ReservationsApimUrl)
        };
        httClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config.ReservationsApimSubscriptionKey);
        httClient.DefaultRequestHeaders.Add("X-Version", "1");

        var apiClient = new OuterApiClient(httClient, config);

        //Act
        var actual = await apiClient.Get<List<string>>(getTestRequest);

        //Assert
        actual.Should().BeEquivalentTo(testObject);
    }

    [Test]
    public void Then_If_It_Is_Not_Successful_An_Exception_Is_Thrown()
    {
        //Arrange
        const string key = "123-abc-567";
        var getTestRequest = new GetTestRequest();
        var config = new ReservationsJobs
        {
            ReservationsBaseUrl = "http://valid-url/",
            ReservationsApimSubscriptionKey = key, 
            ReservationsApimUrl = "https://testing.local/"
        };
        
        var response = new HttpResponseMessage
        {
            Content = new StringContent(""),
            StatusCode = HttpStatusCode.BadRequest
        };

        var httpMessageHandler = SetupMessageHandlerMock(response, $"{config.ReservationsBaseUrl}{getTestRequest.GetUrl}", config.ReservationsApimSubscriptionKey);
        var client = new HttpClient(httpMessageHandler.Object);
        var apiClient = new OuterApiClient(client, config);

        //Act Assert
        var result = async () => await apiClient.Get<List<string>>(getTestRequest);
        result.Should().ThrowAsync<InvalidOperationException>();
    }

    private class GetTestRequest : IGetApiRequest
    {
        public string GetUrl => "test-url/get";
    }

    private static Mock<HttpMessageHandler> SetupMessageHandlerMock(HttpResponseMessage response, string url, string key)
    {
        var httpMessageHandler = new Mock<HttpMessageHandler>();

        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(c =>
                    c.Method.Equals(HttpMethod.Get)
                    && c.Headers.Contains("Ocp-Apim-Subscription-Key")
                    && c.Headers.GetValues("Ocp-Apim-Subscription-Key").Single().Equals(key)
                    && c.Headers.Contains("X-Version")
                    && c.Headers.GetValues("X-Version").Single().Equals("1")
                    && c.RequestUri.AbsoluteUri.Equals(url)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        return httpMessageHandler;
    }
}