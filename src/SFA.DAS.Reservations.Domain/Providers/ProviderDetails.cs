using System;
using SFA.DAS.Apprenticeships.Api.Types.Providers;

namespace SFA.DAS.Reservations.Domain.Providers
{
    public class ProviderDetails
    {
        public uint ProviderId { get; set; }
        public bool IsHigherEducationInstitute { get; set; }
        public string ProviderName { get; set; }
        public bool CurrentlyNotStartingNewApprentices { get; set; }
        public bool IsEmployerProvider { get; set; }
        public string Uri { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool NationalProvider { get; set; }
        public string Website { get; set; }
        public bool HasParentCompanyGuarantee { get; set; }
        public bool IsNew { get; set; }
        public bool IsLevyPayerOnly { get; set; }

        public static implicit operator ProviderDetails(Provider provider)
        {
            return new ProviderDetails
            {
                ProviderId = Convert.ToUInt32(provider.Ukprn),
                IsHigherEducationInstitute = provider.IsHigherEducationInstitute,
                ProviderName = provider.ProviderName,
                CurrentlyNotStartingNewApprentices = provider.CurrentlyNotStartingNewApprentices,
                IsEmployerProvider = provider.IsEmployerProvider,
                Uri = provider.Uri,
                Phone = provider.Phone,
                Email = provider.Email,
                NationalProvider = provider.NationalProvider,
                Website = provider.Website,
                HasParentCompanyGuarantee = provider.HasParentCompanyGuarantee,
                IsNew = provider.IsNew,
                IsLevyPayerOnly = provider.IsLevyPayerOnly
            };
        }
    }
}