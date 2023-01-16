using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Domain.UnitTests.Accounts
{
    public class WhenCastingUserDetailsFromApiType
    {
        [Test, AutoData]
        public void Then_Maps_Matching_Fields(TeamMemberViewModel source)
        {
            UserDetails userDetails = source;
            userDetails.Should().BeEquivalentTo(source, options =>
                    options.ExcludingMissingMembers()
                    .Excluding(c => c.Status)
                    );

            userDetails.Status.Should().Be((byte)source.Status);
        }
    }
}