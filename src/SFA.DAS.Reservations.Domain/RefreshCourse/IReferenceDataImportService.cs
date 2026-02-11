using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.ImportTypes;

namespace SFA.DAS.Reservations.Domain.RefreshCourse;

public interface IReferenceDataImportService
{
    Task<CourseApiResponse> GetCourses();
    Task<ProviderApiResponse> GetProvider(uint ukPrn);
}