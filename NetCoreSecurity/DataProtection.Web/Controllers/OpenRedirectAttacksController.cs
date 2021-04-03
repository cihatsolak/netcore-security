using Microsoft.AspNetCore.Mvc;
using Security.Web.Models.ViewModels;

namespace Security.Web.Controllers
{
    public class OpenRedirectAttacksController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            TempData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            //kullanıcı adı ve şifreyi burada kontrol ettiğimi varsayıyorum.

            string redirectUrl = TempData["ReturnUrl"] as string;

            //Open redirect attacks'a engel.
            if (!Url.IsLocalUrl(redirectUrl)) //Bu metot gelen url'in benim domainime ait olup olmadığını tespit etmekte.
                return Redirect("/");

            return Redirect(redirectUrl);
        }
    }
}
