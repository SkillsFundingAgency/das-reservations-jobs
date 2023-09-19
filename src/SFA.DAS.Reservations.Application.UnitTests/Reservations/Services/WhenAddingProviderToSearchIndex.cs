using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenAddingProviderToSearchIndex
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_All_Reservations_For_AccountLegalEntity(
            uint providerId,
            long accountLegalEntityId,
            [Frozen] Mock<IReservationRepository> mockReservationsRepo,
            ReservationService service)
        {
            await service.AddProviderToSearchIndex(providerId, accountLegalEntityId);

            mockReservationsRepo.Verify(repository => repository.GetAllNonLevyForAccountLegalEntity(accountLegalEntityId), 
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Updates_Index_With_New_Doc_For_Each_AccountLegalEntity(
            uint providerId,
            long accountLegalEntityId,
            Task<List<Reservation>> reservationsFound,
            [Frozen] Mock<IReservationRepository> mockReservationsRepo,
            [Frozen] Mock<IReservationIndexRepository> mockIndexRepo,
            ReservationService service)
        {
            IEnumerable<IndexedReservation> actualIndexedReservations = null;
            mockReservationsRepo
                .Setup(repository => repository.GetAllNonLevyForAccountLegalEntity(accountLegalEntityId))
                .Returns(reservationsFound);
            mockIndexRepo
                .Setup(repository => repository.Add(It.IsAny<IEnumerable<IndexedReservation>>()))
                .Callback((IEnumerable<IndexedReservation> res) => actualIndexedReservations = res);

            await service.AddProviderToSearchIndex(providerId, accountLegalEntityId);

            actualIndexedReservations.Count().Should().Be(reservationsFound.Result.Count);
            foreach (var reservation in reservationsFound.Result)
            {
                var actualReservation = actualIndexedReservations.Single(indexedReservation =>
                    indexedReservation.ReservationId == reservation.Id);

                actualReservation.IndexedProviderId.Should().Be(providerId);
                actualReservation.Id.Should().Be($"{providerId}_{reservation.AccountLegalEntityId}_{reservation.Id}");
                actualReservation.ReservationId.Should().Be(reservation.Id);
                actualReservation.ProviderId.Should().Be(reservation.ProviderId);
                actualReservation.AccountLegalEntityId.Should().Be(reservation.AccountLegalEntityId);
                actualReservation.AccountId.Should().Be(reservation.AccountId);
                actualReservation.AccountLegalEntityName.Should().Be(reservation.AccountLegalEntityName);
                actualReservation.CourseId.Should().Be(reservation.CourseId);
                actualReservation.CourseLevel.Should().Be(reservation.Course.Level);
                actualReservation.CourseTitle.Should().Be(reservation.Course.Title);
                actualReservation.StartDate.Should().Be(reservation.StartDate);
                actualReservation.ExpiryDate.Should().Be(reservation.ExpiryDate);
                actualReservation.CreatedDate.Should().Be(reservation.CreatedDate);
                actualReservation.Status.Should().Be((short)reservation.Status);
                actualReservation.TransferSenderAccountId.Should().Be(reservation.TransferSenderAccountId);
                actualReservation.IsLevyAccount.Should().Be(reservation.IsLevyAccount);
            }
        }

        [Test, MoqAutoData]
        public async  Task Then_Does_Not_Index_If_There_Are_No_Matching_Reservations(
            uint providerId,
            long accountLegalEntityId,
            [Frozen] Mock<IReservationRepository> mockReservationsRepo,
            [Frozen] Mock<IReservationIndexRepository> mockIndexRepo,
            ReservationService service)
        {
            IEnumerable<IndexedReservation> actualIndexedReservations = new List<IndexedReservation>();
            mockReservationsRepo
                .Setup(repository => repository.GetAllNonLevyForAccountLegalEntity(accountLegalEntityId))
                .ReturnsAsync(new List<Reservation>());
            mockIndexRepo
                .Setup(repository => repository.Add(It.IsAny<IEnumerable<IndexedReservation>>()))
                .Callback((IEnumerable<IndexedReservation> res) => actualIndexedReservations = res);

            await service.AddProviderToSearchIndex(providerId, accountLegalEntityId);

            actualIndexedReservations.Count().Should().Be(0);
        }
    }
}