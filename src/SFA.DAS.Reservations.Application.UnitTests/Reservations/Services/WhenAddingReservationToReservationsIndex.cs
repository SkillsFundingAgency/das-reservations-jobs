using System.Threading.Tasks;
using AutoFixture.NUnit3;
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
            IndexedReservation reservation,
            [Frozen] Mock<IProviderPermissionRepository> mockPermissionsRepo,
            ReservationService service)
        {
            await service.AddReservationToReservationsIndex(reservation);

            mockPermissionsRepo.Verify(repository => repository.GetAllForAccountLegalEntity(reservation.AccountLegalEntityId));
        }

        //Then_Updates_Index_With_New_Doc_For_Each_Provider
    }
}