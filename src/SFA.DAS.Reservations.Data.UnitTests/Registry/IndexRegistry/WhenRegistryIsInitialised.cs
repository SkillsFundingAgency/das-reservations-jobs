using System;
using System.Collections.Generic;
using Moq;
using Nest;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Registry;
using SFA.DAS.Reservations.Domain.Configuration;
using IElasticClient = Nest.IElasticClient;

namespace SFA.DAS.Reservations.Data.UnitTests.Registry.IndexRegistry
{
    public class WhenRegistryIsInitialised
    {
        [Test]
        public void ThenGetsExistingRegistryEntry()
        {
            //Arrange
            var clientMock = new Mock<IElasticClient>();

            //Act
            new Data.Registry.IndexRegistry(clientMock.Object, new ReservationJobsEnvironment("LOCAL"));

            //Assert
            clientMock.Verify(c => c.Search(
                It.IsAny<Func<SearchDescriptor<IndexRegistryEntry>, ISearchRequest>>()), Times.Once);
        }

        [Test]
        public void ThenSetsLatestRegistryEntryToCurrent()
        {
            //Arrange
            var latestEntry = new IndexRegistryEntry {Name = "testB", DateCreated = DateTime.Now.AddDays(-1)};
            
            var expectedRegistryEntries = new System.Collections.Generic.List<IndexRegistryEntry>
            {
                new IndexRegistryEntry{Name = "testA", DateCreated = DateTime.Now.AddDays(-5)},
                latestEntry,
                new IndexRegistryEntry{Name = "testC", DateCreated = DateTime.Now.AddDays(-10)}
            };

            var response = new Moq.Mock<Nest.ISearchResponse<IndexRegistryEntry>>();
            response.Setup(x => x.Documents).Returns(expectedRegistryEntries);


            var clientMock = new Mock<IElasticClient>();
            clientMock.Setup(x => x.Search(
                    It.IsAny<Func<SearchDescriptor<IndexRegistryEntry>, ISearchRequest>>()))
                .Returns(response.Object);


            //Act
            var registry = new Data.Registry.IndexRegistry(clientMock.Object, new ReservationJobsEnvironment("LOCAL"));

            //Assert
            Assert.AreEqual(latestEntry.Name, registry.CurrentIndexName);
        }

        [Test]
        public void ThenSetsLatestRegistryEntryToNullIfNoEntries()
        {
            //Arrange
            var response = new Moq.Mock<Nest.ISearchResponse<IndexRegistryEntry>>();
            response.Setup(x => x.Documents).Returns(new List<IndexRegistryEntry>());

            var clientMock = new Mock<IElasticClient>();
            clientMock.Setup(x => x.Search(
                    It.IsAny<Func<SearchDescriptor<IndexRegistryEntry>, ISearchRequest>>()))
                .Returns(response.Object);


            //Act
            var registry = new Data.Registry.IndexRegistry(clientMock.Object, new ReservationJobsEnvironment("LOCAL"));

            //Assert
            Assert.IsNull(registry.CurrentIndexName);
        }
    }
}
