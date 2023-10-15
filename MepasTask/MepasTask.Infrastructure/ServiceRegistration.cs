using Microsoft.Extensions.DependencyInjection;
using MepasTask.App.Interfaces;
using MepasTask.Infrastructure.Repository;

namespace MepasTask.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
           
            services.AddTransient<IProductsRepository, ProductsRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();


        }
    }
}
