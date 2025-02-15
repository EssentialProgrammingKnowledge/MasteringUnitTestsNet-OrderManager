using OrderManager.API.Models;

namespace OrderManager.API.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetById(int id);
        Task<Product> Add(Product product);
        Task<Product> Update(Product product);
        Task<bool> Delete(Product product);
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetProductsByIds(List<int> productIds);
        Task<bool> ProductHasBeenOrdered(int id);
        Task UpdateRange(IEnumerable<Product> products);
    }
}
