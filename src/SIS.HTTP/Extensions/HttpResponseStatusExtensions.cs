using System;
using SIS.HTTP.Enums;

namespace SIS.HTTP.Extensions
{
    public static class HttpResponseStatusExtensions
    {
        private const string NotSupportedStatusCodeExceptionMessage = "Status Code {0} not supported.";

        private static string GetLineByCode(int code)
        {
            switch (code)
            {
                case 200: return "200 OK";
                case 201: return "201 Created";
                case 302: return "302 Found";
                case 303: return "303 See Other";
                case 400: return "400 Bad Request";
                case 401: return "401 Unauthorized";
                case 403: return "403 Forbidden";
                case 404: return "404 Not Found";
                case 500: return "500 Internal Server Error";
            }

            throw new NotSupportedException(string.Format(NotSupportedStatusCodeExceptionMessage, code));
        }
        
        public static string GetResponseLine(this HttpResponseStatusCode httpResponseStatus)
        {
            return GetLineByCode((int) httpResponseStatus);
        }
    }
}
