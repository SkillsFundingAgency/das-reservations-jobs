using Azure;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using SFA.DAS.Reservations.Domain.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Interfaces;

public interface IAzureSearchHelper
{
    Task CreateIndex(string indexName);
    Task DeleteIndex(string indexName);
    Task UploadDocuments(string indexName, IEnumerable<ReservationAzureSearchDocument> documents);
    Task<Response<SearchIndex>> GetIndex(string indexName);
    Task<List<SearchIndex>> GetIndexes();
    Task<SearchAlias> GetAlias(string aliasName);
    Task<SearchResults<ReservationAzureSearchDocument>> GetDocuments(string indexName, string reservationId);
    Task UpdateAlias(string aliasName, string indexName);
    Task DeleteDocument(string indexName, string reservationId);
    Task DeleteDocuments(string indexName, IEnumerable<string> ids);
    Task<SearchResults<ReservationAzureSearchDocument>> SearchDocuments(string indexName, string filter, string[] selectFields);
} 