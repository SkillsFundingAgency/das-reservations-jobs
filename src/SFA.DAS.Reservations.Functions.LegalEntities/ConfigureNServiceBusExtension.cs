using System;
using System.Net;
using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace SFA.DAS.Reservations.Functions.LegalEntities;

public static class ConfigureNServiceBusExtension
{
    private const string EndpointName = "SFA.DAS.Reservations.Jobs.PrlUpdated";
    private const string ErrorEndpointName = $"{EndpointName}-error";

    public static IHostBuilder ConfigureNServiceBus(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseNServiceBus((config, endpointConfiguration) =>
        {
            endpointConfiguration.Transport.SubscriptionRuleNamingConvention = AzureRuleNameShortener.Shorten;

            endpointConfiguration.AdvancedConfiguration.EnableInstallers();
            endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo(ErrorEndpointName);
            endpointConfiguration.AdvancedConfiguration.Conventions()
                .DefiningCommandsAs(IsCommand)
                .DefiningMessagesAs(IsMessage)
                .DefiningEventsAs(IsEvent);

            var decodedLicence = WebUtility.HtmlDecode(config["LevyTransferMatchingFunctions:NServiceBusLicense"]);
            if(!string.IsNullOrWhiteSpace(decodedLicence)) endpointConfiguration.AdvancedConfiguration.License(decodedLicence);
        });

        return hostBuilder;
    }

    private static bool IsMessage(Type t) => t is IMessage || IsDasMessage(t, "Messages");

    private static bool IsEvent(Type t) => t is IEvent || IsDasMessage(t, "Messages.Events");

    private static bool IsCommand(Type t) => t is ICommand || IsDasMessage(t, "Messages.Commands");

    private static bool IsDasMessage(Type t, string namespaceSuffix)
        => t.Namespace != null &&
           t.Namespace.StartsWith("SFA.DAS") &&
           t.Namespace.EndsWith(namespaceSuffix);
}

internal static class AzureRuleNameShortener
{
    private const int AzureServiceBusRuleNameMaxLength = 50;

    public static string Shorten(Type type)
    {
        var ruleName = type.FullName;
        if (ruleName!.Length <= AzureServiceBusRuleNameMaxLength)
        {
            return ruleName;
        }

        var bytes = System.Text.Encoding.Default.GetBytes(ruleName);
        var hash = MD5.HashData(bytes);
        return new Guid(hash).ToString();
    }
}