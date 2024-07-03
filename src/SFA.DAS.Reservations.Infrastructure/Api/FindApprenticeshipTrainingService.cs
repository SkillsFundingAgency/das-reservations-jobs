using System.Threading.Tasks;
using SFA.DAS.Reservations.Application.OuterApi;
using SFA.DAS.Reservations.Application.OuterApi.Requests;
using SFA.DAS.Reservations.Domain.ImportTypes;
using SFA.DAS.Reservations.Domain.RefreshCourse;

namespace SFA.DAS.Reservations.Infrastructure.Api;

public class FindApprenticeshipTrainingService(IOuterApiClient outerApiClient) : IFindApprenticeshipTrainingService
{
    public async Task<StandardApiResponse> GetStandards()
    {
        return await outerApiClient.Get<StandardApiResponse>(new GetStandardsRequest()).ConfigureAwait(false);
    }

    public async Task<ProviderApiResponse> GetProvider(uint ukPrn)
    {
        return await outerApiClient.Get<ProviderApiResponse>(new GetProviderRequest(ukPrn));
    }
}