using DataProtection.Web.Middlewares;
using DataProtection.Web.Models.Context;
using DataProtection.Web.Models.Settings;
using DataProtection.Web.Services.DataProtectors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Security.Web.Filters;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

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

            services.AddControllersWithViews(options =>
            {
                //Uygulama seviyesinde her post metodunda token'ı kontrol edecektir.
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddDataProtection();
            services.AddSingleton<IDataProtectorService, DataProtectorService>();

            services.AddScoped(typeof(CheckWhiteListFilter));
            services.Configure<IPListSettings>(Configuration.GetSection(nameof(IPListSettings)));

            services.AddCors(options =>
            {
                //Default - Herkese ve herşeye açık cors tanımlaması
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin(); //Hangi originden gelirse gelsin isteklere cevap ver.
                    policy.AllowAnyHeader(); //Header'da ne olursa olsun api cevap versin.
                    policy.AllowAnyMethod(); //Put, Post, Get, Delete, Patch hangi istek tipi gelirse gelsin, buna izin veriyoruz.
                });

                //Özelleştirilmiş Cors : İzin verilen siteler
                options.AddPolicy("AllowedSites", policy =>
                {
                    policy.WithOrigins("http://localhost:36020", "https://wwww.mysitem.com") //Sadece bu iki site adresinden gelen isteklere izin ver
                          .AllowAnyHeader() //Herhangi bir header'ı olabilir
                          .AllowAnyHeader(); //Herhangi bir metot olabilir

                    policy.AllowCredentials(); //Kimlikle ilgili cookieleri kabul edip etmeyeceğimle  ilgili izinler
                });

                //Özelleştirilmiş Cors : İzin verilen siteler 2
                options.AddPolicy("AllowedSites2", policy =>
                {
                    policy.WithOrigins("https://wwww.mysitem.com") //Bu siteden gelene izin ver
                          .WithHeaders(HeaderNames.ContentType, "x-customer-header"); //Hangi headerları kabul ediceğim?
                });

                //Özelleştirilmiş Cors : İzin verilen siteler 2
                options.AddPolicy("AllowedSites3", policy =>
                {
                    //domain mysite.com olacak, ve bu domainin tüm subdomainlerinden gelen isteği kabul et. -> deneme.mysite.com
                    policy.WithOrigins("https://*.mysitem.com").SetIsOriginAllowedToAllowWildcardSubdomains()
                          .AllowAnyHeader() //Herhangi bir header
                          .AllowAnyMethod(); //Herhangi bir metot
                });

                //Özelleştirilmiş Cors : İzin verilen siteler 2
                options.AddPolicy("AllowedSites4", policy =>
                {
                    //domain www.example.com domaininden gelen isteği kabul et.
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

            app.UseHttpsRedirection(); //Http üzerinden gelen istekleri https'e yönlendiriyorum.
            app.UseHsts(); //Tarayıcıya bana göndereceğin request'leri https üzerinden gönder diyorum.

            app.UseMiddleware<IPSafeMiddleware>(); //Ip Kontrolu
            app.UseMiddleware<QueryStringMiddleware>(); //DataProtector HTTP GET Query String

            app.UseRouting();
            app.UseStaticFiles();

            app.UseCors("AllowedSites"); //Hangi kuralın uygulanacağını middleware'e veerdiğimiz isimle belirleriz.


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
