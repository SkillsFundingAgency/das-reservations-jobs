using Elasticsearch.Net;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.UnitTests.Registry;

public class WhenGettingCurrentIndex
{
    private const string ExpectedEnvironmentName = "test";
    private const string ExpectedIndexRegistryPostfix = "-reservations-index-registry";
    private const string ExpectedReservationIndexLookupName = ExpectedEnvironmentName + ExpectedIndexRegistryPostfix;
    private const string ExpectedLatestReservationIndexName = "test-reservations-35c937c2-f0b1-4a57-9ebb-621a2834ae8b";

    private Mock<IElasticLowLevelClientWrapper> _elasticLowLevelClient;
    private Mock<IElasticSearchQueries> _elasticSearchQueries;

    [SetUp]
    public void Arrange()
    {
        _elasticLowLevelClient = new Mock<IElasticLowLevelClientWrapper>();
        _elasticSearchQueries = new Mock<IElasticSearchQueries>();
            
        const string indexLookUpResponse = @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":[{""_index"":""local-reservations-index-registry"",""_type"":""_doc"",
            ""_id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""_score"":null,""_source"":{""id"":""41444ccb-9687-4d3a-b0d5-295f3c35b153"",""name"":
            """ + ExpectedLatestReservationIndexName + @""",""dateCreated"":""2019-11-06T15:11:00.5385739+00:00""},""sort"":[1573053060538]}]}}";

        const string getCurrentIndexSearchQuery = @"{""from"": 0, ""size"": ""{size}"", ""sort"": { ""dateCreated"": { ""order"": ""desc"" } } }";
        _elasticSearchQueries.Setup(x => x.LastIndexSearchQuery).Returns(getCurrentIndexSearchQuery);
        _elasticSearchQueries.Setup(x => x.ReservationIndexLookupName).Returns(ExpectedIndexRegistryPostfix);
            
        var searchQuery = getCurrentIndexSearchQuery.Replace(@"""{size}""", "1");
            
        _elasticLowLevelClient.Setup(c =>
                c.Search(ExpectedReservationIndexLookupName,
                    searchQuery))
            .ReturnsAsync(new StringResponse(indexLookUpResponse));
    }

    [Test]
    public void Then_The_Current_Index_Name_Is_Returned()
    {
        //Arrange / Act
        var registry = new IndexRegistry(_elasticLowLevelClient.Object,_elasticSearchQueries.Object, new ReservationJobsEnvironment(ExpectedEnvironmentName));
            
        //Assert
        registry.CurrentIndexName.Should().Be(ExpectedLatestReservationIndexName);
    }

    [Test]
    public void Then_If_There_Is_No_Current_Index_Empty_Is_Returned()
    {
        //Arrange / Act
        var registry = new IndexRegistry(_elasticLowLevelClient.Object,_elasticSearchQueries.Object, new ReservationJobsEnvironment("local"));
            
        //Assert
        registry.CurrentIndexName.Should().BeNullOrEmpty();
    }
}