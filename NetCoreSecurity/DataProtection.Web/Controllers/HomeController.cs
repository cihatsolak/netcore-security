using Microsoft.AspNetCore.Mvc;
using Security.Web.Filters;

namespace DataProtection.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// HTTP GET Yönteminde IDataProtector ile veriyi şifreyip çözdüğüm metot
        /// </summary>
        /// <param name="plate"></param>
        /// <param name="customerId"></param>
        /// <param name="tcNumber"></param>
        /// <returns></returns>
        public IActionResult Detail(string plate, int customerId, string tcNumber)
        {
            ViewBag.Plate = plate;
            ViewBag.CustomerId = customerId;
            ViewBag.TcNumber = tcNumber;

            return View();
        }


        /// <summary>
        /// IP control ettiğim Metot
        /// </summary>
        /// <returns></returns>
        [ServiceFilter(typeof(CheckWhiteListFilter))]
        public IActionResult IPControl()
        {
            return View();
        }
    }
}
