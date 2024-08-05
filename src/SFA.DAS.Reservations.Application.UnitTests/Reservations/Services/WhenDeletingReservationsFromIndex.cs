using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenDeletingReservationsFromIndex
    {
        [Test,MoqAutoData]
        public async Task Then_The_Reservations_Are_Deleted_By_Provider_And_AccountLegalEntityId(
            uint ukPrn,
            long accountLegalEntityId,
            [Frozen]Mock<IReservationIndexRepository> indexRepository,
            ReservationService service
            )
        {
            //Act
            await service.DeleteProviderFromSearchIndex(ukPrn, accountLegalEntityId);

            //Assert
            indexRepository.Verify(x=>x.DeleteReservationsFromIndex(ukPrn,accountLegalEntityId), Times.Once);
        }
    }
}
