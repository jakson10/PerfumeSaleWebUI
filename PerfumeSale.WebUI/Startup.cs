using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PerfumeSale.WebUI.ApiServices.Concrete;
using PerfumeSale.WebUI.ApiServices.Interfaces;

namespace PerfumeSale.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddSession();
            services.AddHttpClient<IImageApiService, ImageApiManager>();
            services.AddHttpClient<IBrandApiService, BrandApiManager>();
            services.AddHttpClient<IPerfumeApiService, PerfumeApiManager>();
            services.AddHttpClient<IAuthApiService, AuthApiManager>();
            services.AddHttpClient<IOrderApiService, OrderApiManager>();
            services.AddHttpClient<IOrderDetailApiService, OrderDetailApiManager>();


            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                   .AddCookie(opt =>
                   {
                       opt.Cookie.Name = "PerfumeSaleCookie";
                       opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict; //strict yaparsan baska sayfayla cookie paylasýlmaz
                       opt.Cookie.HttpOnly = true;  //cookie bilgisine ulasamaz document.write ile
                       opt.ExpireTimeSpan = TimeSpan.FromDays(20); //ne kadar süre ayakta kalýcak
                       opt.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest; //istek neyse o sekýlde davran http yada https
                       opt.LoginPath = "/Security/Login";
                       opt.LogoutPath = "/Security/Logout";
                       opt.AccessDeniedPath = "/Security/AccessDenied";
                       opt.SlidingExpiration = true;
                   });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();
            app.UseAuthentication();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Security}/{action=Login}/{id?}");
            });
        }
    }
}
