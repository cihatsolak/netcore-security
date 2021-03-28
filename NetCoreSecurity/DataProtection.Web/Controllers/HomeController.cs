using Microsoft.AspNetCore.Mvc;

namespace DataProtection.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(string plate, int customerId, string tcNumber)
        {
            ViewBag.Plate = plate;
            ViewBag.CustomerId = customerId;
            ViewBag.TcNumber = tcNumber;

            return View();
        }
    }
}
