namespace SFA.DAS.Reservations.Infrastructure.NServiceBus
{
    public static class AzureFunctionsQueueNames
    {
        public const string LegalEntitiesQueue = "SFA.DAS.Reservations.Functions.LegalEntities";
        public const string ProviderPermissionQueue = "SFA.DAS.Reservations.Functions.ProviderPermission";
        public const string ReservationsQueue = "SFA.DAS.Reservations.Functions.Reservations";

    }
}
