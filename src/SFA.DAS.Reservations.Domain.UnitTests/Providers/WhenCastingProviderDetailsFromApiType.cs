using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.Reservations.Domain.Providers;

namespace SFA.DAS.Reservations.Domain.UnitTests.Providers
{
    public class WhenCastingProviderDetailsFromApiType
    {
        [Test, AutoData]
        public void Then_Maps_Matching_Fields(Provider provider)
        {
            ProviderDetails providerDetails = provider;

            providerDetails.Should().BeEquivalentTo(provider, options => 
                options.ExcludingMissingMembers());
            providerDetails.ProviderId.Should().Be(Convert.ToUInt32(provider.Ukprn));
        }
    }
}