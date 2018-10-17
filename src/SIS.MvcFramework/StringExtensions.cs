using System.Net;

namespace SIS.MvcFramework
{
    public static class StringExtensions
    {
        public static string UrlDecode(this string input)
        {
            return WebUtility.UrlDecode(input);
        }

        public static decimal ToDecimal(this string input)
        {
            if (decimal.TryParse(input, out var result))
            {
                return result;
            }

            return default(decimal);
        }
    }
}
