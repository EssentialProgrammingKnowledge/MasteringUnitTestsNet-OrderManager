using OrderManager.API.DTO;

namespace OrderManager.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProducts();
        Task<Result<ProductDTO>> GetProductById(int id);
        Task<Result<ProductDTO>> AddProduct(ProductDTO productDto);
        Task<Result<ProductDTO>> UpdateProduct(ProductDTO productDto);
        Task<Result> DeleteProduct(int id);
    }
}
