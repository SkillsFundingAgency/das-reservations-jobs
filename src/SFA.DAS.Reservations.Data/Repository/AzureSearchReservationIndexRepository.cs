using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Documents;
using SFA.DAS.Reservations.Domain.Interfaces;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Data.Repository;

public class AzureSearchReservationIndexRepository(
    IAzureSearchHelper azureSearchHelper,
    ILogger<AzureSearchReservationIndexRepository> logger,
    ReservationsJobs configuration)
    : IAzureSearchReservationIndexRepository
{
    private const string IndexPrefix = "reservations-";
    private const string AliasName = "reservations";

    public async Task<string> CreateIndex()
    {
        var indexName = $"{IndexPrefix}{DateTime.UtcNow:yyyyMMddHHmmss}";
        await azureSearchHelper.CreateIndex(indexName);
        return indexName;
    }

    public async Task UpdateAlias(string indexName)
    {
        await azureSearchHelper.UpdateAlias(AliasName, indexName);
    }

    public async Task Add(IEnumerable<IndexedReservation> reservations, string? indexName = null)
    {
        var documents = reservations.Select(r => (ReservationAzureSearchDocument)r);

        if (indexName == null)
        {
            var alias = await azureSearchHelper.GetAlias(AliasName);
            if (alias?.Indexes?.FirstOrDefault() == null)
            {
                logger.LogWarning("Alias '{AliasName}' not found or has no indexes. Skipping document upload.", AliasName);
                return;
            }
            indexName = alias.Indexes.First();
        }
        
        await azureSearchHelper.UploadDocuments(indexName, documents);
    }

    public async Task DeleteIndices(uint daysOld)
    {
        var indexes = await azureSearchHelper.GetIndexes();
        var alias = await azureSearchHelper.GetAlias(AliasName);
        var aliasTarget = alias?.Indexes.FirstOrDefault();

        foreach (var index in indexes.Where(i => i.Name.StartsWith(IndexPrefix)))
        {
            if (index.Name == aliasTarget)
            {
                continue;
            }

            var indexDate = DateTime.ParseExact(
                index.Name.Substring(IndexPrefix.Length),
                "yyyyMMddHHmmss",
                System.Globalization.CultureInfo.InvariantCulture);

            if ((DateTime.UtcNow - indexDate).TotalDays > daysOld)
            {
                await azureSearchHelper.DeleteIndex(index.Name);
            }
        }
    }

    public async Task DeleteReservationsFromIndex(uint ukPrn, long accountLegalEntityId)
    {
        var alias = await azureSearchHelper.GetAlias(AliasName);
        if (alias == null)
        {
            return;
        }

        var index = await azureSearchHelper.GetIndex(alias.Indexes.First());
        if (index.Value == null)
        {
            return;
        }

        try
        {
            var filter = $"ProviderId eq {ukPrn} and AccountLegalEntityId eq {accountLegalEntityId}";

            var searchResults = await azureSearchHelper.SearchDocuments(index.Value.Name, filter, ["Id"]);

            var documentIds = searchResults.GetResults().Select(r => r.Document.Id).ToList();

            if (documentIds.Any())
            {
                await azureSearchHelper.DeleteDocuments(index.Value.Name, documentIds);
            }

            logger.LogInformation(
                "Successfully deleted {Count} reservations for ProviderId [{UkPrn}], AccountLegalEntityId [{AccountLegalEntityId}] from index.",
                documentIds.Count, ukPrn, accountLegalEntityId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error deleting reservations for ProviderId [{UkPrn}], AccountLegalEntityId [{AccountLegalEntityId}] from index.",
                ukPrn, accountLegalEntityId);
            throw;
        }
    }

    public async Task SaveReservationStatus(Guid id, ReservationStatus status)
    {
        var alias = await azureSearchHelper.GetAlias(AliasName);
        if (alias == null)
        {
            return;
        }

        var index = await azureSearchHelper.GetIndex(alias.Indexes.First());
        if (index.Value == null)
        {
            return;
        }

        try
        {
            var document = await azureSearchHelper.GetDocument(index.Value.Name, id.ToString());
            if (document?.Value == null)
            {
                logger.LogWarning("Reservation {ReservationId} was not found in the index", id);
                return;
            }

            var updatedDocument = document.Value;
            updatedDocument.Status = (int)status;

            await azureSearchHelper.UploadDocuments(index.Value.Name, [updatedDocument]);

            logger.LogInformation("Successfully updated status for reservation {ReservationId} to {Status}", id, status);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating status for reservation {ReservationId} to {Status}", id, status);
            throw;
        }
    }
} 