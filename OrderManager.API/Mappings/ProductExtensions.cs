using OrderManager.API.DTO;
using OrderManager.API.Models;

namespace OrderManager.API.Mappings
{
    public static class ProductExtensions
    {
        public static ProductDTO AsDto(this Product product)
        {
            return new ProductDTO(
                product.Id,
                product.ProductName,
                product.Price,
                product.IsDigital,
                !product.IsDigital ? product.ProductStock.AsDto() : null
            );
        }

        public static ProductStockDTO AsDto(this ProductStock? productStock)
        {
            return new ProductStockDTO(productStock?.Quantity ?? 0);
        }
    }
}
