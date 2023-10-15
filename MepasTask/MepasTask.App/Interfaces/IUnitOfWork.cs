

namespace MepasTask.App.Interfaces
{
    public interface IUnitOfWork
    {
        IProductsRepository Products { get; }
        
    }
}