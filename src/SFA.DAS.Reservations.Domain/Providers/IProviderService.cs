using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Providers
{
    public interface IProviderService
    {
        Task<ProviderDetails> GetDetails(uint providerId);
    }
}