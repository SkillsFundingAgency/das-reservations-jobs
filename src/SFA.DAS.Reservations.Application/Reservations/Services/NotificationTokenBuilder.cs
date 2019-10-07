using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Providers;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public interface INotificationTokenBuilder
    {
        Task<Dictionary<string, string>> BuildTokens(ReservationCreatedEvent createdEvent);
    }

    public class NotificationTokenBuilder : INotificationTokenBuilder
    {
        private readonly IProviderService _providerService;

        public NotificationTokenBuilder(IProviderService providerService)
        {
            _providerService = providerService;
        }

        public async Task<Dictionary<string, string>> BuildTokens(ReservationCreatedEvent createdEvent)
        {
            var provider = await _providerService.GetDetails(createdEvent.ProviderId.Value);
            var startDateDescription = $"{createdEvent.StartDate:MMM yyyy} to {createdEvent.EndDate:MMM yyyy}";
            var courseDescription = $"{createdEvent.CourseName} level {createdEvent.CourseLevel}";
            var manageUrl = "todo";

            return new Dictionary<string, string>
            {
                {TokenKeyNames.ProviderName,  provider.ProviderName},
                {TokenKeyNames.StartDateDescription, startDateDescription},
                {TokenKeyNames.CourseDescription, courseDescription},
                {TokenKeyNames.ManageUrl, manageUrl}
            };
        }
    }
}