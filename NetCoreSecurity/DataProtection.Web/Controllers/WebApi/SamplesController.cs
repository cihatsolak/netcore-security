using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace DataProtection.Web.Controllers.WebApi
{

    [EnableCors("AllowedSites4")] //Controller seviyesinde bir cors ayarı gerçekleştirdik.
    [ApiController, Route("api/[controller]/[action]")]
    public class SamplesController : ControllerBase
    {
        [EnableCors("AllowedSites3")] //Method seviyesinde bir cors ayarı
        [HttpGet]
        public IActionResult GetKdv()
        {
            return Ok(18);
        }

        [DisableCors] //Controller seviesinde cors gerçekleştirdiğimizde bu cors ayarının bu metot için geçerli olmamasını istiyoruz.
        [HttpGet]
        public IActionResult GetOtv()
        {
            return Ok(18);
        }
    }
}
