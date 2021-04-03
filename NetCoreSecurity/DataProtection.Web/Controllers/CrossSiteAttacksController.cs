using Microsoft.AspNetCore.Mvc;
using Security.Web.Models.ViewModels;
using System.Text.Encodings.Web;

namespace Security.Web.Controllers
{
    public class CrossSiteAttacksController : Controller
    {
        /// <summary>
        /// Html içerikleri encode etmek için. (.cshtml tarafında @ directive i bu işlemi gerçekleştiriyor.)
        /// </summary>
        private readonly HtmlEncoder _htmlEncoder;
        /// <summary>
        /// Javascript Encoder
        /// </summary>
        private readonly JavaScriptEncoder _javaSriptEncoder;
        /// <summary>
        /// Url Encoder
        /// </summary>
        private readonly UrlEncoder _urlEncoder;

        public CrossSiteAttacksController(
            HtmlEncoder htmlEncoder,
            JavaScriptEncoder javaSriptEncoder,
            UrlEncoder urlEncoder)
        {
            _htmlEncoder = htmlEncoder;
            _javaSriptEncoder = javaSriptEncoder;
            _urlEncoder = urlEncoder;
        }

        [HttpGet]
        public IActionResult ReflectedXSS()
        {
            //Örnek Cookie Bilgisi Oluşturuyorum.
            HttpContext.Response.Cookies.Append("username", "cihatsolak");
            HttpContext.Response.Cookies.Append("password", "123456");

            return View();
        }

        [HttpPost]
        public IActionResult ReflectedXSS(VehicleAddViewModel vehicleAddViewModel)
        {
            ViewBag.Year = vehicleAddViewModel.Year;
            ViewBag.Color = vehicleAddViewModel.Color;
            ViewBag.Plate = vehicleAddViewModel.Plate;

            return View();
        }

        [HttpGet]
        public IActionResult StoredXSS()
        {
            if (System.IO.File.Exists("comment.txt"))
            {
                ViewBag.Comments = System.IO.File.ReadAllLines("comment.txt");
            }

            return View();
        }

        [HttpPost]
        public IActionResult StoredXSS(CommentViewModel commentViewModel)
        {
            string text = string.Concat(commentViewModel.Name, "--", commentViewModel.Comment, "\n");
            System.IO.File.AppendAllText("comment.txt", text);

            return RedirectToAction(nameof(StoredXSS));
        }

        /// <summary>
        /// Encodeları tanıtmak için kullandığım action
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EncodeSample(string name)
        {
            //Url üzerinde encode edilmesi için
            string encodedName = _urlEncoder.Encode(name);
            
            return View();
        }
    }
}
