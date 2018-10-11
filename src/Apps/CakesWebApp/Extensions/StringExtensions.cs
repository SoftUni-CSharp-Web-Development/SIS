using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CakesWebApp.Extensions
{
    public static class StringExtensions
    {
        public static string UrlDecode(this string input)
        {
            return WebUtility.UrlDecode(input);
        }
    }
}
