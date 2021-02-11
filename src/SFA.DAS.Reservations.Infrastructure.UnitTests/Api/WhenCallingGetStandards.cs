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
    public class WhenCallingGetStandards
    {
        [Test, MoqAutoData]
        public async Task Then_The_Endpoint_Is_Called_With_Api_Key_And_Standards_Returned(
            Domain.ImportTypes.StandardApiResponse importStandards,
            Mock<IOptions<ReservationsJobs>> configuration, 
            string baseUrl,
            string apiKey)
        {
            //Arrange
            baseUrl = "https://" + baseUrl;
            configuration.Setup(x => x.Value.ReservationsApimSubscriptionKey).Returns(apiKey);
            configuration.Setup(x => x.Value.ReservationsApimUrl).Returns(baseUrl);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(importStandards)),
                StatusCode = HttpStatusCode.Accepted
            };
            var httpMessageHandler = HttpMessageHandlerBuilder.SetupMessageHandlerMock(response, new Uri(baseUrl + "/trainingcourses"), apiKey);
            var client = new HttpClient(httpMessageHandler);
            var apprenticeshipService = new FindApprenticeshipTrainingService(client, configuration.Object);
            
            //Act
            var standards = await apprenticeshipService.GetStandards();
            
            //Assert
            standards.Should().BeEquivalentTo(importStandards);
        }
        
        [Test, AutoData]
        public void Then_If_It_Is_Not_Successful_An_Exception_Is_Thrown(
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
            var httpMessageHandler = HttpMessageHandlerBuilder.SetupMessageHandlerMock(response, new Uri(baseUrl + "/trainingcourses"), apiKey);
            var client = new HttpClient(httpMessageHandler);
            var apprenticeshipService = new FindApprenticeshipTrainingService(client, configuration.Object);
            
            //Act Assert
            Assert.ThrowsAsync<HttpRequestException>(() => apprenticeshipService.GetStandards());
        }
    }
}