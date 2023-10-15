using MepasTask.App.Interfaces;
using MepasTask.Infrastructure;
using MepasTask.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using MepasTask.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MepasTask.API
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
            // services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddInfrastructure();
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSingleton<ITempDataDictionaryFactory, TempDataDictionaryFactory>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Mepas Task - WebApi",
                });
               
            });

           
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            #region Swagger
            app.UseSwagger();
            app.UseSwagger();
            

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mepas Task - WebApi");

                c.DocumentTitle = "My API";
                c.DocExpansion(DocExpansion.None);
                c.EnableFilter();
                c.EnableDeepLinking();
                c.EnableValidator(null);
                c.DisplayRequestDuration();
                c.DefaultModelExpandDepth(0);
                c.OAuthClientSecret(Configuration.GetSection("AppSetting")["Secret"]);
            });
            #endregion
          
        }

    }

}
