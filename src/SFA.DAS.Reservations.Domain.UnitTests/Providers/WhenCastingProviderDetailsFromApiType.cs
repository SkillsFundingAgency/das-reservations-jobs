using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.ImportTypes;
using SFA.DAS.Reservations.Domain.Providers;

namespace SFA.DAS.Reservations.Domain.UnitTests.Providers
{
    public class WhenCastingProviderDetailsFromApiType
    {
        [Test, AutoData]
        public void Then_Maps_Matching_Fields(ProviderApiResponse source)
        {
            ProviderDetails providerDetails = source;

            providerDetails.Should().BeEquivalentTo(source);
        }
    }
}