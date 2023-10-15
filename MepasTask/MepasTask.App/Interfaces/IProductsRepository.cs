using MepasTask.Models;

namespace MepasTask.App.Interfaces
{
    public interface IProductsRepository// : IGenericRepository<ProductsModel>
    {
        Task<IReadOnlyList<ProductsModel>> GetAllAsync(string filePath);
        
    }
}