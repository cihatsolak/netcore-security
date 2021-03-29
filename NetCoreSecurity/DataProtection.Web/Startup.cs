using DataProtection.Web.Middlewares;
using DataProtection.Web.Models.Context;
using DataProtection.Web.Services.DataProtectors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin(); //Hangi originden gelirse gelsin isteklere cevap ver.
                    policy.AllowAnyHeader(); //Header'da ne olursa olsun api cevap versin.
                    policy.AllowAnyMethod(); //Put, Post, Get, Delete, Patch hangi istek tipi gelirse gelsin, buna izin veriyoruz.
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

            app.UseCors();

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
