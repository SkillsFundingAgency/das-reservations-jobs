﻿using System.Collections.Generic;
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
            var provider = await _providerService.GetDetails(createdEvent.ProviderId.Value);
            var startDateDescription = $"{createdEvent.StartDate:MMM yyyy} to {createdEvent.EndDate:MMM yyyy}";
            var courseDescription = $"{createdEvent.CourseName} level {createdEvent.CourseLevel}";
            var hashedAccountId = _encodingService.Encode(createdEvent.AccountId, EncodingType.AccountId);

            return new Dictionary<string, string>
            {
                {TokenKeyNames.ProviderName,  provider.ProviderName},
                {TokenKeyNames.StartDateDescription, startDateDescription},
                {TokenKeyNames.CourseDescription, courseDescription},
                {TokenKeyNames.HashedAccountId, hashedAccountId}
            };
        }

        public async Task<Dictionary<string, string>> BuildReservationDeletedTokens(ReservationDeletedEvent deletedEvent)
        {
            var provider = await _providerService.GetDetails(deletedEvent.ProviderId.Value);
            var startDateDescription = $"{deletedEvent.StartDate:MMM yyyy} to {deletedEvent.EndDate:MMM yyyy}";
            var courseDescription = $"{deletedEvent.CourseName} level {deletedEvent.CourseLevel}";
            var hashedAccountId = _encodingService.Encode(deletedEvent.AccountId, EncodingType.AccountId);

            return new Dictionary<string, string>
            {
                {TokenKeyNames.ProviderName,  provider.ProviderName},
                {TokenKeyNames.StartDateDescription, startDateDescription},
                {TokenKeyNames.CourseDescription, courseDescription},
                {TokenKeyNames.HashedAccountId, hashedAccountId}
            };
        }
    }
}