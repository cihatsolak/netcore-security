using DataProtection.Web.Services.DataProtectors;
using DataProtection.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataProtection.Web.Middlewares
{
    /// <summary>
    /// Constants.QueryStringPrefix == q
    /// Query stringde q harfi geçen HttpGet istekleri şifrelenmiştir. Haricindeki get istekleri normal isteklerdir.
    /// </summary>
    public class QueryStringMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public QueryStringMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext httpContext, IDataProtectorService dataProtectorService)
        {
            if (httpContext.Request.Method == HttpMethod.Get.Method && httpContext.Request.Query.ContainsKey(Constants.QueryStringPrefix))
            {
                bool isTimeLimited = httpContext.Request.Query.ContainsKey(Constants.TimeStringPrefix); //Query string'e expire minutes verilmiş mi?

                string queryStrings = httpContext.Request.Query[Constants.QueryStringPrefix].ToString();
                string unprotected = dataProtectorService.Unprotect(queryStrings, isTimeLimited);

                string urlDecode = WebUtility.UrlDecode(unprotected);
                httpContext.Request.QueryString = QueryString.Create(QueryHelpers.ParseQuery(urlDecode));
            }

            await _requestDelegate(httpContext);
        }
    }
}
