using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Repository.Common.Validation;

namespace Repository.Internal
{
    internal static class UrlUtilities
    {
        public static Dictionary<string, string> ParseQueryParameters(string url)
        {
            Verify.NotNullOrEmpty(url, nameof(url));

            int index = url.IndexOf('?');
            if (index != -1)
            {
                url = url.Substring(index + 1);
            }

            var keyValuePairs = url.Split('&');

            var result = new Dictionary<string, string>();

            foreach (var kvp in keyValuePairs)
            {
                int equalsIndex = kvp.IndexOf('=');
                // There should be exactly one occurrence of = in between &s in a well-formed query string.
                Verify.Argument(equalsIndex != -1 && equalsIndex == kvp.LastIndexOf('='), nameof(url));

                var key = kvp.Substring(0, equalsIndex);
                var value = WebUtility.UrlDecode(kvp.Substring(equalsIndex + 1));
                result.Add(key, value);
            }

            return result;
        }
    }
}