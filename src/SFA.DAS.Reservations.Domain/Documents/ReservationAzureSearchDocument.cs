using Azure.Search.Documents.Indexes;
using System;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.Documents;

public class ReservationAzureSearchDocument
{
    public static implicit operator ReservationAzureSearchDocument(IndexedReservation source)
    {
        return new ReservationAzureSearchDocument
        {
            Id = source.Id,
            ReservationId = source.ReservationId.ToString(),
            AccountId = source.AccountId,
            IsLevyAccount = source.IsLevyAccount,
            CreatedDate = new DateTimeOffset(source.CreatedDate),
            StartDate = source.StartDate.HasValue ? new DateTimeOffset(source.StartDate.Value) : null,
            ExpiryDate = source.ExpiryDate.HasValue ? new DateTimeOffset(source.ExpiryDate.Value) : null,
            Status = (int)source.Status,
            CourseId = source.CourseId,
            CourseTitle = source.CourseTitle,
            CourseLevel = source.CourseLevel,
            CourseDescription = source.CourseDescription,
            AccountLegalEntityId = source.AccountLegalEntityId,
            ProviderId = source.ProviderId.HasValue ? (int)source.ProviderId.Value : 0,
            AccountLegalEntityName = source.AccountLegalEntityName,
            TransferSenderAccountId = source.TransferSenderAccountId,
            UserId = source.UserId?.ToString(),
            IndexedProviderId = (int)source.IndexedProviderId,
            ReservationPeriod = source.ReservationPeriod
        };
    }

    [SimpleField(IsKey = true)]
    public string Id { get; set; }

    [SimpleField(IsFilterable = true)]
    public string ReservationId { get; set; }

    [SimpleField(IsFilterable = true)]
    public long AccountId { get; set; }

    [SimpleField(IsFilterable = true)]
    public bool IsLevyAccount { get; set; }

    [SimpleField(IsSortable = true)]
    public DateTimeOffset CreatedDate { get; set; }

    [SimpleField(IsSortable = true)]
    public DateTimeOffset? StartDate { get; set; }

    [SimpleField(IsSortable = true)]
    public DateTimeOffset? ExpiryDate { get; set; }

    [SimpleField(IsFilterable = true)]
    public int Status { get; set; }

    [SimpleField(IsFilterable = true)]
    public string CourseId { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
    public string CourseTitle { get; set; }

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public int? CourseLevel { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
    public string CourseDescription { get; set; }

    [SimpleField(IsFilterable = true)]
    public long AccountLegalEntityId { get; set; }

    [SimpleField(IsFilterable = true)]
    public int ProviderId { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
    public string AccountLegalEntityName { get; set; }

    [SimpleField(IsFilterable = true)]
    public long? TransferSenderAccountId { get; set; }

    [SimpleField(IsFilterable = true)]
    public string UserId { get; set; }

    [SimpleField(IsFilterable = true)]
    public int IndexedProviderId { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true, IsFacetable = true)]
    public string ReservationPeriod { get; set; }
} 