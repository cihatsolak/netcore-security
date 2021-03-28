using DataProtection.Web.Models.Context;
using DataProtection.Web.Models.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataProtection.Web.Controllers
{
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

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Color")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Color")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
