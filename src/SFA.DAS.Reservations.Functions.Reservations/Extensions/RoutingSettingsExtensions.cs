﻿using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.Reservations.Functions.Reservations.Extensions;

public static class RoutingSettingsExtensions
{
    private const string NotificationsMessageHandler = "SFA.DAS.Notifications.MessageHandlers";

    public static void AddRouting(this RoutingSettings routingSettings)
    {
        routingSettings.RouteToEndpoint(typeof(SendEmailCommand), NotificationsMessageHandler);
    }
}