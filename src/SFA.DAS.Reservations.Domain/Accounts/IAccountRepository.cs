using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Domain.Accounts
{
    public interface IAccountRepository
    {
        Task Add(Account account);
        Task UpdateName(Account account);
    }
}