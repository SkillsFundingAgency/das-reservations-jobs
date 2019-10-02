using AutoFixture;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.UnitTests.Customisations
{
    public class UsersCanReceiveNotificationsAndWithRoleCustomisation : ICustomization
    {
        private readonly string _role;

        public UsersCanReceiveNotificationsAndWithRoleCustomisation(string role)
        {
            _role = role;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<UserDetails>(composer => composer
                .With(user => user.Role, _role)
                .With(user => user.CanReceiveNotifications, true));
        }
    }
}