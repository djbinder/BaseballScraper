using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using BaseballScraper.Client;

namespace BaseballScraper.Infrastructure
{
    internal static class NameValueCollectionExtensions
    {

        internal static string GetOrThrowUnexpectedResponse (this NameValueCollection collection, string key)
        {
            var value = collection[key];
            if (string.IsNullOrWhiteSpace (value))
            {
                throw new UnexpectedResponseException (key);
            }
            return value;
        }
    }
}
