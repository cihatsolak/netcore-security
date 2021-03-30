using DataProtection.Web.Models.Context;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataProtection.Web.Controllers
{
    /// <summary>
    /// DataProtector çalışması için oluşturulmuş ef controller
    /// </summary>
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDataProtector _dataProtector; //Veriyi şifrelemek için kullanırız.
        private readonly ITimeLimitedDataProtector _timeLimitedDataProtector; //Şifrelediğimiz veriye ömür biçmek için kullanırız.

        public ProductController(AppDbContext context, IDataProtectionProvider dataProtectionProvider) //IDataProtectionProvider ile IDataProtector'ı dolduracağız.
        {
            _context = context;
            /* 
             * CreateProtector içerisinde vereceğim isim Unique'dir. DataProtector'ları birbirinden ayırmak için kullanırız.
             * Farklı bir controller içerisinde de DataProtector kullanabileceğimizden ötürü, bunları birbirinden ayırmak mahiyetinde isimlendirme yapıyoruz.
             */
            _dataProtector = dataProtectionProvider.CreateProtector(nameof(ProductController));
            _timeLimitedDataProtector = _dataProtector.ToTimeLimitedDataProtector(); //Şifrelediğimiz veriye ömür biçmek için kullanırız.

            //_dataProtector = dataProtectionProvider.CreateProtector(GetType().FullName); | GetType().FullName ile otomatik olarak da isimlendirebiliriz.
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();

            /*
             * Protect: Koru
             * UnProtect: Korumayı Kaldır
             */
            products.ForEach((product) =>
            {
                //product.EncryptedId = _dataProtector.Protect($"{product.Id}"); //Zaman bağımsız şifreleme
                product.EncryptedId = _timeLimitedDataProtector.Protect($"{product.Id}", TimeSpan.FromSeconds(20)); //Zaman bağımlı şifreleme (Tarih, Saat, Dakika, Saniye)
            });

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // int decryptedId = int.Parse(_dataProtector.Unprotect(id)); //Zaman bağımsız şifre çözme işlemi
            int decryptedId = int.Parse(_timeLimitedDataProtector.Unprotect(id)); //Zaman bağımlı şifre çözme

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == decryptedId);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
