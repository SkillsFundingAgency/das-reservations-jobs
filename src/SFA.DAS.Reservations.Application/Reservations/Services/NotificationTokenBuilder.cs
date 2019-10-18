using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public interface INotificationTokenBuilder
    {
        Task<Dictionary<string, string>> BuildReservationCreatedTokens(ReservationCreatedEvent createdEvent);
        Task<Dictionary<string, string>> BuildReservationDeletedTokens(ReservationDeletedEvent deletedEvent);
    }

    public class NotificationTokenBuilder : INotificationTokenBuilder
    {
        private readonly IProviderService _providerService;
        private readonly IEncodingService _encodingService;

        public NotificationTokenBuilder(
            IProviderService providerService,
            IEncodingService encodingService)
        {
            _providerService = providerService;
            _encodingService = encodingService;
        }

        public async Task<Dictionary<string, string>> BuildReservationCreatedTokens(ReservationCreatedEvent createdEvent)
        {
            return new Dictionary<string, string>
            {
                {TokenKeyNames.ProviderName,  await GetProviderName(createdEvent.ProviderId.Value)},
                {TokenKeyNames.StartDateDescription, GenerateStartDateDescription(createdEvent.StartDate, createdEvent.EndDate)},
                {TokenKeyNames.CourseDescription, GenerateCourseDescription(createdEvent.CourseName, createdEvent.CourseLevel)},
                {TokenKeyNames.HashedAccountId, GetHashedAccountId(createdEvent.AccountId)}
            };
        }

        public async Task<Dictionary<string, string>> BuildReservationDeletedTokens(ReservationDeletedEvent deletedEvent)
        {
            return new Dictionary<string, string>
            {
                {TokenKeyNames.ProviderName,  await GetProviderName(deletedEvent.ProviderId.Value)},
                {TokenKeyNames.StartDateDescription, GenerateStartDateDescription(deletedEvent.StartDate, deletedEvent.EndDate)},
                {TokenKeyNames.CourseDescription, GenerateCourseDescription(deletedEvent.CourseName, deletedEvent.CourseLevel)},
                {TokenKeyNames.HashedAccountId, GetHashedAccountId(deletedEvent.AccountId)}
            };
        }

        private async Task<string> GetProviderName(uint providerId)
        {
            var provider = await _providerService.GetDetails(providerId);
            return provider.ProviderName;
        }

        private string GenerateStartDateDescription(DateTime startDate, DateTime endDate)
        {
            return $"{startDate:MMM yyyy} to {endDate:MMM yyyy}";
        }

        private string GenerateCourseDescription(string courseName, string courseLevel)
        {
            return $"{courseName} level {courseLevel}";
        }

        private string GetHashedAccountId(long accountId)
        {
            return _encodingService.Encode(accountId, EncodingType.AccountId);
        }
    }
}