using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Notifications;
using SFA.DAS.Reservations.Domain.Providers;

namespace SFA.DAS.Reservations.Application.Reservations.Services;

public class NotificationTokenBuilder(
    IProviderService providerService,
    IEncodingService encodingService,
    ReservationsJobs config)
    : INotificationTokenBuilder
{
    public async Task<Dictionary<string, string>> BuildTokens<T>(T notificationEvent) where T : INotificationEvent
    {
        return new Dictionary<string, string>
        {
            { TokenKeyNames.ProviderName, await GetProviderName(notificationEvent.ProviderId.Value) },
            { TokenKeyNames.StartDateDescription, GenerateStartDateDescription(notificationEvent.StartDate, notificationEvent.EndDate) },
            { TokenKeyNames.CourseDescription, GenerateCourseDescription(notificationEvent.CourseName, notificationEvent.CourseLevel) },
            { TokenKeyNames.HashedAccountId, GetHashedAccountId(notificationEvent.AccountId) },
            { TokenKeyNames.BaseUrl, config.ReservationsBaseUrl }
        };
    }

    private async Task<string> GetProviderName(uint providerId)
    {
        var provider = await providerService.GetDetails(providerId);
        return provider.Name;
    }

    private static string GenerateStartDateDescription(DateTime startDate, DateTime endDate)
    {
        return $"{startDate:MMM yyyy} to {endDate:MMM yyyy}";
    }

    private static string GenerateCourseDescription(string courseName, string courseLevel)
    {
        return $"{courseName} level {courseLevel}";
    }

    private string GetHashedAccountId(long accountId)
    {
        return encodingService.Encode(accountId, EncodingType.AccountId);
    }
}