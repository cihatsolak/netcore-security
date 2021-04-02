using Microsoft.AspNetCore.Mvc;
using Security.Web.Models.ViewModels;

namespace Security.Web.Controllers
{
    public class CrossSiteAttacksController : Controller
    {
        [HttpGet]
        public IActionResult VehicleAdd()
        {
            //Örnek Cookie Bilgisi Oluşturuyorum.
            HttpContext.Response.Cookies.Append("username", "cihatsolak");
            HttpContext.Response.Cookies.Append("password", "123456");

            return View();
        }

        [HttpPost]
        public IActionResult VehicleAdd(VehicleAddViewModel vehicleAddViewModel)
        {
            ViewBag.Year = vehicleAddViewModel.Year;
            ViewBag.Color = vehicleAddViewModel.Color;
            ViewBag.Plate = vehicleAddViewModel.Plate;

            return View();
        }
    }
}
