using DataProtection.Web.Models.Settings;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;

namespace Security.Web.Filters
{
    public class CheckWhiteListFilter : IActionFilter
    {
        private readonly IPListSettings _iPListSettings;

        public CheckWhiteListFilter(IOptions<IPListSettings> ipListSettings)
        {
            _iPListSettings = ipListSettings.Value;
        }

        /// <summary>
        /// Action'dan yani metotdan çıktından sonra
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Action'a yani metot girmeden önce
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            IPAddress requestIPAddress = context.HttpContext.Connection.RemoteIpAddress; //Gelen isteğin ip adresi

            var isWhiteList = _iPListSettings.WhiteList.Any(item => IPAddress.Parse(item).Equals(requestIPAddress));
            if (!isWhiteList)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
        }
    }
}
