using System.Threading.Tasks;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.OuterApi.Requests;
using SFA.DAS.Reservations.Domain.ImportTypes;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Infrastructure.Api;

public class ReferenceDataImportService(IOuterApiClient outerApiClient) : IReferenceDataImportService
{
    public async Task<CourseApiResponse> GetCourses()
    {
        return await outerApiClient.Get<CourseApiResponse>(new GetCoursesRequest()).ConfigureAwait(false);
    }

    public async Task<ProviderApiResponse> GetProvider(uint ukPrn)
    {
        return await outerApiClient.Get<ProviderApiResponse>(new GetProviderRequest(ukPrn));
    }
}