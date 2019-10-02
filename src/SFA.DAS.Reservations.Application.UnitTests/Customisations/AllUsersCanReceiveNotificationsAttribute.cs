using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using AutoFixture.NUnit3;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.UnitTests.Customisations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AllUsersCanReceiveNotificationsAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter.ParameterType != typeof(List<UserDetails>))
            {
                throw new ArgumentException(nameof(parameter));
            }

            return new ArrangeAllUsersCanReceiveNotificationsCustomisation();
        }
    }

    public class ArrangeAllUsersCanReceiveNotificationsCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<UserDetails>(composer => composer
                .With(user => user.CanReceiveNotifications, true));
        }
    }
}