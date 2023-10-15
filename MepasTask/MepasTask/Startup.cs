using MepasTask.App.Interfaces;
using MepasTask.Infrastructure;
using MepasTask.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Data;

namespace MepasTask.UI
{
    public class Startup
    {
        public IConfiguration configRoot { get; }

        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            configRoot = configuration;
            _environment = environment;
        }
        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddControllersWithViews();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IProductsRepository, ProductsRepository>();
            services.AddInfrastructure();
            services.AddHttpClient();
            services.AddRazorPages();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
               .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
               .AddDataAnnotationsLocalization();



            services.AddDistributedMemoryCache();
           

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();

            app.UseRouting();
            //
            app.UseDeveloperExceptionPage();
           
            app.UseHttpsRedirection();
            app.UseStaticFiles();
          
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseCookiePolicy();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}"); // Default route Login/Index olarak ayarlandı
            });




            app.Run();
           
        }

   }

}

