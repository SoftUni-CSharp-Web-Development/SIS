using System;
using System.Collections.Generic;
using System.Linq;
using SIS.HTTP.Common;
using SIS.HTTP.Cookies;
using SIS.HTTP.Enums;
using SIS.HTTP.Exceptions;
using SIS.HTTP.Extensions;
using SIS.HTTP.Headers;
using SIS.HTTP.Sessions;

namespace SIS.HTTP.Requests
{
    public class HttpRequest : IHttpRequest
    {
        private const char HttpRequestUrlQuerySeparator = '?';

        private const char HttpRequestUrlFragmentSeparator = '#';

        private const string HttpRequestHeaderNameValueSeparator = ": ";

        private const string HttpRequestCookiesSeparator = "; ";

        private const char HttpRequestCookieNameValueSeparator = '=';

        private const char HttpRequestParameterSeparator = '&';

        private const char HttpRequestParameterNameValueSeparator = '=';

        public HttpRequest(string requestString)
        {
            CoreValidator.ThrowIfNullOrEmpty(requestString, nameof(requestString));

            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();

            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        public IHttpSession Session { get; set; }

        private bool IsValidRequestLine(string[] requestLine)
        {
            return requestLine.Length == 3 
                   && requestLine[2].ToLower() 
                   != GlobalConstants.HttpOneProtocolFragment;
        }

        private bool IsValidRequestQueryString(string queryString, string[] queryParameters)
        {
            return !(string.IsNullOrEmpty(queryString) || queryParameters.Length < 1);
        }

        private void ParseRequestMethod(string[] requestLine)
        {
            bool parseMethodResult = Enum.TryParse<HttpRequestMethod>(requestLine[0].Capitalize(), out HttpRequestMethod parsedMethod);

            if(!parseMethodResult) throw new BadRequestException();

            this.RequestMethod = parsedMethod;
        }

        private void ParseRequestUrl(string[] requestLine)
        {
            this.Url = requestLine[1];
        }

        private void ParseRequestPath()
        {
            this.Path = 
                this.Url.Split(new[] {HttpRequestUrlQuerySeparator, HttpRequestUrlFragmentSeparator}, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        private void ParseHeaders(string[] requestContent)
        {
            int currentIndex = 0;

            while (!string.IsNullOrEmpty(requestContent[currentIndex]))
            {
                string[] headerArguments = requestContent[currentIndex++].Split(HttpRequestHeaderNameValueSeparator);

                this.Headers.Add(new HttpHeader(headerArguments[0], headerArguments[1]));
            }

            if (!this.Headers.ContainsHeader(GlobalConstants.HostHeaderKey))
            {
                throw new BadRequestException();
            }
        }

        private void ParseCookies()
        {
            if (!this.Headers.ContainsHeader(HttpHeader.Cookie)) return;

            string cookiesString = this.Headers.GetHeader(HttpHeader.Cookie).Value;

            if (string.IsNullOrEmpty(cookiesString)) return;

            string[] splitCookies = cookiesString.Split(HttpRequestCookiesSeparator);

            foreach (var splitCookie in splitCookies)
            {
                string[] cookieParts = splitCookie.Split(HttpRequestCookieNameValueSeparator, 2, StringSplitOptions.RemoveEmptyEntries);

                if (cookieParts.Length != 2) continue;

                string key = cookieParts[0];
                string value = cookieParts[1];

                this.Cookies.Add(new HttpCookie(key, value, false));
            }
        }

        private void ParseQueryParameters()
        {
            if (!this.Url.Contains('?'))
            {
                return;
            }

            string queryString = this.Url
                .Split(new [] {'?', '#'}, StringSplitOptions.None)[1];

            if (string.IsNullOrWhiteSpace(queryString))
            {
                return;
            }

            string[] queryParameters = queryString.Split('&');

            if (!this.IsValidRequestQueryString(queryString, queryParameters))
            {
                throw new BadRequestException();
            }

            foreach (var queryParameter in queryParameters)
            {
                string[] parameterArguments = queryParameter
                    .Split('=', StringSplitOptions.RemoveEmptyEntries);

                this.QueryData.Add(parameterArguments[0], parameterArguments[1]);
            }
        }

        private void ParseFormDataParameters(string formData)
        {
            if (string.IsNullOrEmpty(formData))
            {
                return;
            }

            string[] formDataParams = formData.Split(HttpRequestParameterSeparator);

            foreach (var formDataParameter in formDataParams)
            {
                string[] parameterArguments = formDataParameter
                    .Split(HttpRequestParameterNameValueSeparator);

                this.FormData.Add(parameterArguments[0], parameterArguments[1]);
            }
        }

        private void ParseRequestParameters(string formData)
        {
            this.ParseQueryParameters();
            this.ParseFormDataParameters(formData);
        }

        private void ParseRequest(string requestString)
        {
            string[] splitRequestContent = requestString
                .Split(new[] {GlobalConstants.HttpNewLine}, StringSplitOptions.None);

            string[] requestLine = splitRequestContent[0].Trim().
                Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsValidRequestLine(requestLine))
            {
                throw new BadRequestException();
            }

            this.ParseRequestMethod(requestLine);
            this.ParseRequestUrl(requestLine);
            this.ParseRequestPath();

            this.ParseHeaders(splitRequestContent.Skip(1).ToArray());
            this.ParseCookies();

            this.ParseRequestParameters(splitRequestContent[splitRequestContent.Length - 1]);
        }
    }
}
