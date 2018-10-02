namespace SIS.HTTP.Headers
{
    using Common;

    public class HttpHeader
    {
        public const string Cookie = "Cookie";

        public const string ContentType = "Content-Type";

        public const string ContentLength = "Content-Length";

        public const string ContentDisposition = "Content-Disposition";

        public const string Authorization = "Authorization";

        public const string Host = "Host";

        public const string Location = "Location";

        public HttpHeader(string key, string value)
        {
            CoreValidator.ThrowIfNullOrEmpty(key, nameof(key));
            CoreValidator.ThrowIfNullOrEmpty(value, nameof(value));

            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"{this.Key}: {this.Value}";
        }
    }
}
