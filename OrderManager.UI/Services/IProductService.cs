using OrderManager.UI.Models;

namespace OrderManager.UI.Services
{
    public interface IProductService
    {
        Task<Result<List<ProductDTO>>> GetAll();
        Task<Result<ProductDTO?>> GetById(int id);
        Task<Result> Add(ProductDTO product);
        Task<Result> Update(ProductDTO product);
        Task<Result> Delete(int id);
    }
}
