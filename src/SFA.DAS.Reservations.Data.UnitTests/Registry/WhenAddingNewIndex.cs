using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.UnitTests.Registry
{
    public class WhenAddingNewIndex
    {
        private const string ExpectedEnvironmentName = "test";
        private const string ExpectedIndexRegistryPostfix = "-reservations-index-registry";
        private const string ExpectedReservationIndexLookupName = ExpectedEnvironmentName + ExpectedIndexRegistryPostfix;
        private const string ExpectedLatestReservationIndexName = "test-reservations-35c937c2-f0b1-4a57-9ebb-621a2834ae8b";
        
        private Mock<IElasticLowLevelClientWrapper> _elasticLowLevelClient;
        private Mock<IElasticSearchQueries> _elasticSearchQueries;
        private IndexRegistry _registry;
        private string _createdResponse;

        [SetUp]
        public void Arrange()
        {
            var indexLookUpResponse =  @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":""local-reservations-index-registry"",""_type"":""_doc"",
            ""_id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""_score"":null,""_source"":{""id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""name"":
            """ + ExpectedLatestReservationIndexName + @""",""dateCreated"":""2019-11-06T15:11:00.5385739+00:00""},""sort"":[1573053060538]}]}}";

            _createdResponse = @"{""_index"" : ""test-index"", ""_type"" : ""_doc"", ""_id"" : ""1"", ""_version"" : 1, ""result"" :
            ""created"", ""_shards"" : { ""total"" : 2, ""successful"" : 1, ""failed"" : 0 },  ""_seq_no"" : 0, ""_primary_term"" : 1 }";
            
            _elasticLowLevelClient = new Mock<IElasticLowLevelClientWrapper>();
            _elasticSearchQueries = new Mock<IElasticSearchQueries>();
            
            _elasticSearchQueries.Setup(x => x.LastIndexSearchQuery).Returns("Get index");
            _elasticSearchQueries.Setup(x => x.ReservationIndexLookupName).Returns(ExpectedIndexRegistryPostfix);
            
            _elasticLowLevelClient.Setup(c =>
                    c.Search(ExpectedReservationIndexLookupName,
                        It.IsAny<string>()))
                .ReturnsAsync(new StringResponse(indexLookUpResponse));
            
            
            
            _registry = new IndexRegistry(_elasticLowLevelClient.Object, _elasticSearchQueries.Object, new ReservationJobsEnvironment(ExpectedEnvironmentName));
        }

        [Test]
        public async Task Then_The_New_Index_Is_Created_With_The_Passed_In_Data()
        {
            //Arrange
            const string indexName = "new-index";
            Guid test;
            _elasticLowLevelClient.Setup(c => c.Create
                (ExpectedReservationIndexLookupName, It.Is<string>(s=>Guid.TryParse(s, out test)), It.IsAny<string>()))
                .ReturnsAsync(JsonConvert.DeserializeObject<ElasticSearchResponse>(_createdResponse));
            
            //Act
            await _registry.Add(indexName);
            
            //Assert
            _registry.CurrentIndexName.Should().Be(indexName);
        }
    }
}