using MepasTask.App.Interfaces;

namespace MepasTask.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProductsRepository Products { get; }

        public UnitOfWork(
             IProductsRepository products
            
        )
        {
            Products = products;
           
        }
    }
}
