using Moq;
using Nest;
using NUnit.Framework;

namespace SFA.DAS.Reservations.Data.UnitTests.Registry.IndexRegistry
{
    public class WhenIndexesAreDeleted
    {
        private Data.Registry.IndexRegistry _registry;
        private Mock<IElasticClient> _client;

        [SetUp]
        public void Init()
        {
            _client = new Mock<IElasticClient>();
            _registry = new Data.Registry.IndexRegistry(_client.Object);
        }

        [Test]
        public void ThenShouldDeleteAllIndicesOldThanExpiryDate()
        {

        }

        [Test]
        public void  ThenShouldDeleteAllIndexRegistryEntriesThatHaveBeenDeleted()
        {

        }
    }
}
