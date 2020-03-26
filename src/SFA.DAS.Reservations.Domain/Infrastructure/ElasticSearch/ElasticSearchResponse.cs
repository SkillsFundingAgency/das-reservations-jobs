using Elasticsearch.Net;

namespace SFA.DAS.Reservations.Domain.Infrastructure.ElasticSearch
{
    public class ElasticSearchResponse : IElasticsearchResponse
    {
        private ServerError _serverError;
        private Error _error;
        private int? _statusCode;
        public ServerError ServerError
        {
            get
            {
                if (this._serverError != null)
                    return this._serverError;
                if (this._error == null)
                    return (ServerError) null;
                this._serverError = new ServerError(this._error, this._statusCode);
                return this._serverError;
            }
        }
        internal int? StatusCode
        {
            get
            {
                return this._statusCode;
            }
            set
            {
                this._statusCode = value;
                this._serverError = (ServerError) null;
            }
        }
        internal Error Error
        {
            get
            {
                return this._error;
            }
            set
            {
                this._error = value;
                this._serverError = (ServerError) null;
            }
        }
        
        public Shards _shards { get; set; }
        public string _index { get; set; }
        public string _id { get; set; }
        public int _version { get; set; }
        public int _seq_no { get; set; }
        public int _primary_term { get; set; }
        public string result { get; set; }
        
        public bool TryGetServerErrorReason(out string reason)
        {
            reason = ServerError?.Error?.ToString();
            return !string.IsNullOrEmpty(reason);
        }

        public IApiCallDetails ApiCall { get; set; }
    }
}