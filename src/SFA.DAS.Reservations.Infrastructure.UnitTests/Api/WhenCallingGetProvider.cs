using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Infrastructure.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.Api
{
    public class WhenCallingGetProvider
    { 
        [Test, MoqAutoData]
        public async Task Then_The_Endpoint_Is_Called_With_Api_Key_And_Providers_Returned(
            uint ukPrn,
            string baseUrl,
            string apiKey,
            Domain.ImportTypes.ProviderApiResponse providerApiResponse,
            Mock<IOptions<ReservationsJobs>> configuration)
        {
            //Arrange
            baseUrl = "https://" + baseUrl;
            configuration.Setup(x => x.Value.ReservationsApimSubscriptionKey).Returns(apiKey);
            configuration.Setup(x => x.Value.ReservationsApimUrl).Returns(baseUrl);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(providerApiResponse)),
                StatusCode = HttpStatusCode.Accepted
            };
            var httpMessageHandler = HttpMessageHandlerBuilder.SetupMessageHandlerMock(response, new Uri(baseUrl), apiKey);
            var client = new HttpClient(httpMessageHandler);
            var apprenticeshipService = new FindApprenticeshipTrainingService(client, configuration.Object);
            
            //Act
            var standards = await apprenticeshipService.GetProvider(ukPrn);
            
            //Assert
            standards.Should().BeEquivalentTo(providerApiResponse);
        }
        
        [Test, AutoData]
        public void And_Response_Not_200_Then_Exception_Is_Thrown(
            uint ukPrn,
            string baseUrl,
            string apiKey,
            Mock<IOptions<ReservationsJobs>> configuration)
        {
            //Arrange
            baseUrl = "https://" + baseUrl;
            configuration.Setup(x => x.Value.ReservationsApimSubscriptionKey).Returns(apiKey);
            configuration.Setup(x => x.Value.ReservationsApimUrl).Returns(baseUrl);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = HttpStatusCode.BadRequest
            };
            var httpMessageHandler = HttpMessageHandlerBuilder.SetupMessageHandlerMock(response, new Uri(baseUrl), apiKey);
            var client = new HttpClient(httpMessageHandler);
            var apprenticeshipService = new FindApprenticeshipTrainingService(client, configuration.Object);
            
            //Act Assert
            Assert.ThrowsAsync<HttpRequestException>(() => apprenticeshipService.GetProvider(ukPrn));
        }
    }
}