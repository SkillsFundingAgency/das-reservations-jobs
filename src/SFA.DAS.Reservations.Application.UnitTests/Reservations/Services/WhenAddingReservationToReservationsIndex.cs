using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Reservations.Services;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Reservations.Services
{
    public class WhenAddingReservationToReservationsIndex
    {
        [Test, MoqAutoData]
        public async Task Then_Finds_All_Providers_With_Permission_To_View_This_Reservation(
            Reservation reservation,
            [Frozen] Mock<IProviderPermissionRepository> mockPermissionsRepo,
            ReservationService service)
        {
            await service.AddReservationToReservationsIndex(reservation);

            mockPermissionsRepo.Verify(repository => repository.GetAllForAccountLegalEntity(reservation.AccountLegalEntityId));
        }

        [Test, MoqAutoData]
        public async Task Then_Updates_Index_With_New_Doc_For_Each_Provider(
            Reservation reservation,
            List<Domain.Entities.ProviderPermission> permissions,
            [Frozen] Mock<IProviderPermissionRepository> mockPermissionsRepo,
            [Frozen] Mock<IReservationIndexRepository> mockIndexRepo,
            ReservationService service)
        {
            IEnumerable<IndexedReservation> actualIndexedReservations = null;
            mockPermissionsRepo
                .Setup(repository => repository.GetAllForAccountLegalEntity(reservation.AccountLegalEntityId))
                .Returns(permissions);
            mockIndexRepo
                .Setup(repository => repository.Add(It.IsAny<IEnumerable<IndexedReservation>>()))
                .Callback((IEnumerable<IndexedReservation> res) => actualIndexedReservations = res);

            await service.AddReservationToReservationsIndex(reservation);

            actualIndexedReservations.Count().Should().Be(permissions.Count);
            foreach (var providerPermission in permissions)
            {
                var actualReservation = actualIndexedReservations.Single(indexedReservation =>
                    indexedReservation.IndexedProviderId == providerPermission.ProviderId);

                actualReservation.IndexedProviderId.Should().Be((uint)providerPermission.ProviderId);
                actualReservation.Id.Should().Be($"{providerPermission.ProviderId}_{reservation.AccountLegalEntityId}_{reservation.Id}");
                actualReservation.ReservationId.Should().Be(reservation.Id);
                actualReservation.ProviderId.Should().Be(reservation.ProviderId);
                actualReservation.AccountLegalEntityId.Should().Be(reservation.AccountLegalEntityId);
                actualReservation.AccountId.Should().Be(reservation.AccountId);
                actualReservation.AccountLegalEntityName.Should().Be(reservation.AccountLegalEntityName);
                actualReservation.CourseId.Should().Be(reservation.CourseId);
                actualReservation.CourseLevel.Should().Be(reservation.CourseLevel);
                actualReservation.CourseTitle.Should().Be(reservation.CourseName);
                actualReservation.StartDate.Should().Be(reservation.StartDate);
                actualReservation.ExpiryDate.Should().Be(reservation.EndDate);
                actualReservation.CreatedDate.Should().Be(reservation.CreatedDate);
                actualReservation.Status.Should().Be((short)reservation.Status);
                actualReservation.TransferSenderAccountId.Should().BeNull();
                actualReservation.IsLevyAccount.Should().Be(false);
            }
        }
    }
}