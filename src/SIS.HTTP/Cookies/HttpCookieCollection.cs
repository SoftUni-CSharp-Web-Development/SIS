using System.Collections;

namespace SIS.HTTP.Cookies
{
    using System.Collections.Generic;
    using Common;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private const string HttpCookieStringSeparator = "; ";

        private readonly Dictionary<string, HttpCookie> cookies;

        public HttpCookieCollection()
        {
            this.cookies = new Dictionary<string, HttpCookie>();
        }

        public void Add(HttpCookie cookie)
        {
            CoreValidator.ThrowIfNull(cookie, nameof(cookie));
            this.cookies.Add(cookie.Key, cookie);
        }

        public bool ContainsCookie(string key)
        {
            CoreValidator.ThrowIfNull(key, nameof(key));
            return this.cookies.ContainsKey(key);
        }

        public HttpCookie GetCookie(string key)
        {
            CoreValidator.ThrowIfNull(key, nameof(key));
            return this.cookies.GetValueOrDefault(key, null);
        }

        public bool HasCookies()
        {
            return this.cookies.Count > 0;
        }

        public IEnumerator<HttpCookie> GetEnumerator()
        {
            foreach (var cookie in this.cookies)
            {
                yield return cookie.Value;
            }
        }

        public override string ToString()
        {
            return string.Join(HttpCookieStringSeparator, this.cookies.Values);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
