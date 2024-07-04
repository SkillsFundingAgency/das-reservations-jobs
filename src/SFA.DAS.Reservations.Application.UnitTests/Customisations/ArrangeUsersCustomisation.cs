using AutoFixture;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.UnitTests.Customisations
{
    public class ArrangeUsersCustomisation : ICustomization
    {
        private readonly string _role;
        private readonly bool _canReceiveNotifications;

        public ArrangeUsersCustomisation(string role, bool canReceiveNotifications)
        {
            _role = role;
            _canReceiveNotifications = canReceiveNotifications;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<TeamMember>(composer => composer
                .With(user => user.Role, _role)
                .With(user => user.CanReceiveNotifications, _canReceiveNotifications));
        }
    }
}