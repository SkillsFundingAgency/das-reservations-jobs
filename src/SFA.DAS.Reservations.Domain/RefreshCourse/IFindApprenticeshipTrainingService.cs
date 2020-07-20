using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.ImportTypes;

namespace SFA.DAS.Reservations.Domain.RefreshCourse
{
    public interface IFindApprenticeshipTrainingService
    {
        Task<StandardApiResponse> GetStandards();
    }
}