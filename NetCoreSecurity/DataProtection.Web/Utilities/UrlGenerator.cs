using DataProtection.Web.Services.DataProtectors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace DataProtection.Web.Utilities
{
    /// <summary>
    /// View tarafında güvenli bir queryString oluşturmak için kullanacağımız sınıf
    /// </summary>
    public static class UrlGenerator
    {
        /// <summary>
        /// DefaultDataProtectorProvider servisini çağırıyorum
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static IDataProtectorService GetProtector(ActionContext context)
        {
            return ServiceProviderServiceExtensions.GetService<IDataProtectorService>(context.HttpContext.RequestServices);
        }

        public static string ToQueryString(this NameValueCollection values)
        {
            var array = (from key in values.AllKeys
                         from value in values.GetValues(key)
                         select string.Format("{0}={1}", WebUtility.UrlEncode(key), WebUtility.UrlEncode(value))).ToArray();

            return "?" + string.Join("&", array);
        }

        public static string GenerateProtectedUrl(this IUrlHelper urlHelper,
            ActionContext context,
            string actionName,
            string controllerName,
            object routeValues,
            int minute = 0)
        {
            var values = new NameValueCollection();
            var dictionary = new RouteValueDictionary(routeValues);

            foreach (KeyValuePair<string, object> entry in dictionary)
            {
                if (entry.Value != null)
                    values.Add(entry.Key, entry.Value.ToString());
            }

            string protectedValue = GetProtector(context).Protect(values.ToQueryString(), minute);
            string protectedQueryString = WebUtility.UrlEncode(protectedValue);

            string url = string.Concat("/", controllerName, "/", actionName, "?", Constants.QueryStringPrefix, "=", protectedQueryString);
            return url;
        }
    }
}
