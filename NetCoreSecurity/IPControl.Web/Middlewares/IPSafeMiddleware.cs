using IPControl.Web.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IPControl.Web.Middlewares
{
    public class IPSafeMiddleware
    {
        //Gelen isteği yakalıyorum. (RequestDelegate)
        private readonly RequestDelegate _next;
        private readonly IPListSettings _iPListSettings;

        public IPSafeMiddleware(RequestDelegate next, IOptions<IPListSettings> ipListSettings)
        {
            _next = next;
            _iPListSettings = ipListSettings.Value;
        }

        /// <summary>
        /// Her istekle beraber invoke isminde bir metot çalışır. Invoke ismi rastgele bir isim değildir.
        /// </summary>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            IPAddress requestIPAddress = httpContext.Connection.RemoteIpAddress; //Gelen isteğin ip adresi

            var isWhiteList = _iPListSettings.WhiteList.Any(item => IPAddress.Parse(item).Equals(requestIPAddress));
            if (!isWhiteList)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            //Gelen isteğe devam ettiriyorum.
            await _next(httpContext);
        }
    }
}
