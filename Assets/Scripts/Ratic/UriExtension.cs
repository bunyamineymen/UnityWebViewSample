using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;

namespace Ratic
{
    public static class UriExtension
    {
        public static Uri AddQueryParameters(this Uri uri, Dictionary<string, string> queryParams)
        {
            if (queryParams == null || queryParams.Count == 0)
            {
                return uri;
            }

            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }
        
        //alternative for above depending on .net version
        public static string ToQueryString(this NameValueCollection nvc)
        {
            IEnumerable<string> segments = from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format("{0}={1}", 
                    WebUtility.UrlEncode(key),
                    WebUtility.UrlEncode(value));
            return "?" + string.Join("&", segments);
        }
    }
}