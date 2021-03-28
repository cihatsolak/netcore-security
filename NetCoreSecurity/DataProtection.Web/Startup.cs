using DataProtection.Web.Middlewares;
using DataProtection.Web.Models.Context;
using DataProtection.Web.Models.Settings;
using DataProtection.Web.Services.DataProtectors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Security.Web.Filters;

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
            services.AddScoped(typeof(CheckWhiteListFilter));

            services.Configure<IPListSettings>(Configuration.GetSection(nameof(IPListSettings)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<IPSafeMiddleware>(); //Ip Kontrolü
            app.UseMiddleware<QueryStringMiddleware>(); //DataProtector HTTP GET Query String

            app.UseRouting();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
