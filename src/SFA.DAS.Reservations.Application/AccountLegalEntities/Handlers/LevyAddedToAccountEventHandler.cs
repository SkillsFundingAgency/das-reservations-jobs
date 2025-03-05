using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Accounts;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Handlers;

public class LevyAddedToAccountEventHandler : ILevyAddedToAccountEventHandler
{
    private readonly IAccountsService _accountLegalEntitiesService;
    private readonly ILogger<LevyAddedToAccountEventHandler> _logger;

    public LevyAddedToAccountEventHandler(IAccountsService accountLegalEntitiesService,
        ILogger<LevyAddedToAccountEventHandler> logger)
    {
        _accountLegalEntitiesService = accountLegalEntitiesService;
        _logger = logger;
    }

    public async Task Handle(LevyAddedToAccount levyAddedToAccountEvent)
    {
        try
        {
            await _accountLegalEntitiesService.UpdateLevyStatus(levyAddedToAccountEvent.AccountId, true);
        }
        catch (DbUpdateException e)
        {
            _logger.LogWarning("Could not update agreement status to signed", e);
            throw;
        }
    }
}