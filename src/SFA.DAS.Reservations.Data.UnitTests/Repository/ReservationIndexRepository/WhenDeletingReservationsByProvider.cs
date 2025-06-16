using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository.ReservationIndexRepository
{
    public class WhenDeletingReservationsByProvider
    {
        private const string ExpectedEnvironmentName = "local";
        private const string ExpectedCurrentIndexName = "local-reservation-latest";
        private const string ExpectedDeleteQuery = @"{ukPrn}_{accountLegalEntityId}_";
        
        private Mock<IElasticLowLevelClientWrapper> _elasticClientWrapper;
        private Data.Repository.ElasticReservationIndexRepository _elasticReservationIndexRepository;
        private Mock<IIndexRegistry> _indexRegistry;
        private Mock<IElasticSearchQueries> _elasticSearchQueries;

        [SetUp]
        public void Arrange()
        {
            _elasticClientWrapper = new Mock<IElasticLowLevelClientWrapper>();
            _indexRegistry = new Mock<IIndexRegistry>();
            _elasticSearchQueries = new Mock<IElasticSearchQueries>();
            _elasticSearchQueries.Setup(x => x.DeleteReservationsByQuery).Returns(ExpectedDeleteQuery);

            _indexRegistry.Setup(x => x.CurrentIndexName).Returns(ExpectedCurrentIndexName);
            
            _elasticReservationIndexRepository = new Data.Repository.ElasticReservationIndexRepository(_elasticClientWrapper.Object,_indexRegistry.Object, _elasticSearchQueries.Object, new ReservationJobsEnvironment(ExpectedEnvironmentName));
        }

        [Test]
        public async Task Then_The_Client_Is_Called_With_AccountLegalEntityId_And_UkPrn_Substituted_In_Query()
        {
            //Arrange
            var expectedUkPrn = 1111122u;
            var expectedAccountLegalEntityId = 5566;
            var expectedQuery = ExpectedDeleteQuery.Replace("{ukPrn}", expectedUkPrn.ToString())
                .Replace("{accountLegalEntityId}", expectedAccountLegalEntityId.ToString());
            
            //Act
            await _elasticReservationIndexRepository.DeleteReservationsFromIndex(expectedUkPrn, expectedAccountLegalEntityId);
            
            //Assert
            _elasticClientWrapper.Verify(x=>x.DeleteByQuery(ExpectedCurrentIndexName, expectedQuery), Times.Once);
        }
    }
}