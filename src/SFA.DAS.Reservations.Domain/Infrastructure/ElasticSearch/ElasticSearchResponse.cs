using Elasticsearch.Net;

namespace SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch
{
    public class ElasticSearchResponse : IElasticsearchResponse
    {
        public Shards _shards { get; set; }
        public string _index { get; set; }
        public string _id { get; set; }
        public int _version { get; set; }
        public int _seq_no { get; set; }
        public int _primary_term { get; set; }
        public string result { get; set; }
        
        public bool TryGetServerErrorReason(out string reason)
        {
            throw new System.NotImplementedException();
        }

        public IApiCallDetails ApiCall { get; set; }
    }
}