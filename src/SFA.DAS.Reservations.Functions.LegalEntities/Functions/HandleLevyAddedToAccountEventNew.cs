﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Functions.LegalEntities.Functions;

public class HandleLevyAddedToAccountEventNew(ILevyAddedToAccountHandler handler,
    ILogger<LevyAddedToAccountEvent> log) : IHandleMessages<LevyAddedToAccount>
{
    public async Task Handle(LevyAddedToAccount message, IMessageHandlerContext context)
    {
        log.LogInformation($"NServiceBus LevyAddedToAccountEvent trigger function started execution at: {DateTime.Now} for ${nameof(message.AccountId)}:${message.AccountId}");
        await handler.Handle(message);
        log.LogInformation($"NServiceBus LevyAddedToAccountEvent trigger function finished execution at: {DateTime.Now} for ${nameof(message.AccountId)}:${message.AccountId}");
    }
}