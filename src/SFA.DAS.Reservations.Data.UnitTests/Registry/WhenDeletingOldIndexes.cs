using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.UnitTests.Registry
{
    public class WhenDeletingOldIndexes
    {
        private const string ExpectedEnvironmentName = "test";
        private const string ExpectedIndexRegistryPostfix = "-reservations-index-registry";
        private const string ExpectedReservationIndexLookupName = ExpectedEnvironmentName + ExpectedIndexRegistryPostfix;
        
        private Mock<IElasticLowLevelClientWrapper> _elasticLowLevelClient;
        private Mock<IElasticSearchQueries> _elasticSearchQueries;
        private IndexRegistry _registry;
        private List<TestIndexEntry> _listOfEntry;
        private Mock<LowLevelIndicesNamespace> _indices;
        private string _indexLookUpResponse;

        [SetUp]
        public void Arrange()
        {
            
            _listOfEntry = new List<TestIndexEntry>
            {
                CreateTestIndexEntry("test1",DateTime.UtcNow),
                CreateTestIndexEntry("test2",DateTime.UtcNow.AddDays(-1)),
                CreateTestIndexEntry("test3",DateTime.UtcNow.AddDays(-2), true)
            };
            
            _indexLookUpResponse =  @"{""took"":0,""timed_out"":false,""_shards"":{""total"":1,""successful"":1,""skipped"":0,""failed"":0},""hits"":{""total"":
            {""value"":3,""relation"":""eq""},""max_score"":null,""hits"":" + JsonConvert.SerializeObject(_listOfEntry) + @"}}";
            
            _elasticLowLevelClient = new Mock<IElasticLowLevelClientWrapper>();
            _elasticSearchQueries = new Mock<IElasticSearchQueries>();

            _indices = new Mock<LowLevelIndicesNamespace>();

            _elasticSearchQueries.Setup(x => x.ReservationIndexLookupName).Returns(ExpectedIndexRegistryPostfix);
            _elasticSearchQueries.Setup(x => x.LastIndexSearchQuery).Returns("Get index");

            _elasticLowLevelClient.Setup(c =>
                    c.Search(ExpectedReservationIndexLookupName,
                        It.IsAny<string>()))
                .ReturnsAsync(new StringResponse(_indexLookUpResponse));
            
            _registry = new IndexRegistry(_elasticLowLevelClient.Object, _elasticSearchQueries.Object, new ReservationJobsEnvironment(ExpectedEnvironmentName));
        }

        [Test]
        public async Task Then_The_Current_List_Of_Indexes_Are_Returned()
        {
            //Act
            await _registry.DeleteOldIndices(2);
            
            //Act
            _elasticLowLevelClient.Verify(c =>
                    c.Search(
                        ExpectedReservationIndexLookupName,
                        It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public async Task Then_The_Index_Registry_Is_Updated_Removing_Old_Indexes()
        {
            //Arrange
            var expectedIdToBeDeleted = _listOfEntry.FirstOrDefault(c => c._source.Name.Equals("test3"))._id;
            
            //Act
            await _registry.DeleteOldIndices(2);
            
            //Assert
            _elasticLowLevelClient.Verify(x => 
                x.DeleteDocument(ExpectedReservationIndexLookupName, expectedIdToBeDeleted)
                , Times.Once);
        }

        [Test]
        public async Task Then_The_Indexes_Are_Deleted_That_Are_Of_The_Defined_Period()
        {
            //Act
            await _registry.DeleteOldIndices(2);
            
            //Assert
            _elasticLowLevelClient.Verify(x => 
                    x.DeleteIndices("test3")
                , Times.Once);
        }

        private TestIndexEntry CreateTestIndexEntry(string indexName, DateTime dateCreated, bool delete = false)
        {
            var id = Guid.NewGuid();
            return new TestIndexEntry
            {
                _id = id.ToString(),
                _index = ExpectedReservationIndexLookupName,
                _source = new IndexRegistryEntry
                {
                    Id = id,
                    Name = indexName,
                    DateCreated = dateCreated
                }
            };
        }

        public class TestIndexEntry
        {
            public string _index { get; set; }
            public string _type => "_doc";
            public string _id { get; set; }
            public string _score => null;
            public IndexRegistryEntry _source { get; set; }
            public List<long> sort { get; set; }
    
        }
    }

}