using AutoFixture;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.UnitTests.Customisations
{
    public class ArrangeUsersCustomisation(string role, bool canReceiveNotifications) : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<TeamMember>(composer => composer
                .With(user => user.Role, role)
                .With(user => user.CanReceiveNotifications, canReceiveNotifications));
        }
    }
}