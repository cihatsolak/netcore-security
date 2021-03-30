using DataProtection.Web.Middlewares;
using DataProtection.Web.Models.Context;
using DataProtection.Web.Services.DataProtectors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;

namespace DataProtection.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            });

            services.AddControllersWithViews();

            services.AddDataProtection();
            services.AddSingleton<IDataProtectorService, DataProtectorService>();


            services.AddCors(options =>
            {
                //Default - Herkese ve her�eye a��k cors tan�mlamas�
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin(); //Hangi originden gelirse gelsin isteklere cevap ver.
                    policy.AllowAnyHeader(); //Header'da ne olursa olsun api cevap versin.
                    policy.AllowAnyMethod(); //Put, Post, Get, Delete, Patch hangi istek tipi gelirse gelsin, buna izin veriyoruz.
                });

                //�zelle�tirilmi� Cors : �zin verilen siteler
                options.AddPolicy("AllowedSites", policy =>
                {
                    policy.WithOrigins("http://localhost:36020", "https://wwww.mysitem.com") //Sadece bu iki site adresinden gelen isteklere izin ver
                          .AllowAnyHeader() //Herhangi bir header'� olabilir
                          .AllowAnyHeader(); //Herhangi bir metot olabilir

                    policy.AllowCredentials(); //Kimlikle ilgili cookieleri kabul edip etmeyece�imle  ilgili izinler
                });

                //�zelle�tirilmi� Cors : �zin verilen siteler 2
                options.AddPolicy("AllowedSites2", policy =>
                {
                    policy.WithOrigins("https://wwww.mysitem.com") //Bu siteden gelene izin ver
                          .WithHeaders(HeaderNames.ContentType, "x-customer-header"); //Hangi headerlar� kabul edice�im?
                });

                //�zelle�tirilmi� Cors : �zin verilen siteler 2
                options.AddPolicy("AllowedSites3", policy =>
                {
                    //domain mysite.com olacak, ve bu domainin t�m subdomainlerinden gelen iste�i kabul et. -> deneme.mysite.com
                    policy.WithOrigins("https://*.mysitem.com").SetIsOriginAllowedToAllowWildcardSubdomains()
                          .AllowAnyHeader() //Herhangi bir header
                          .AllowAnyMethod(); //Herhangi bir metot
                });

                //�zelle�tirilmi� Cors : �zin verilen siteler 2
                options.AddPolicy("AllowedSites4", policy =>
                {
                    //domain www.example.com domaininden gelen iste�i kabul et.
                    policy.WithOrigins("https://www.example.com")
                          .WithMethods(HttpMethods.Get, HttpMethods.Post) //Sadece GET, POST isteklerine izin ver.
                          .AllowAnyHeader() //Herhangi bir header
                          .AllowAnyMethod(); //Herhangi bir metot
                });

            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();

            app.UseCors("AllowedSites"); //Hangi kural�n uygulanaca��n� middleware'e veerdi�imiz isimle belirleriz.

            app.UseMiddleware<QueryStringMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
