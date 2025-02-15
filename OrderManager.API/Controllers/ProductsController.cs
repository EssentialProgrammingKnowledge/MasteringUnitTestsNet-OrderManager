using Microsoft.AspNetCore.Mvc;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Services;

namespace OrderManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController
        (
            IProductService productService
        )
        : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            return await productService.GetAllProducts();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            return (await productService.GetProductById(id)).ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(ProductDTO product)
        {
            var result = await productService.AddProduct(product);
            return result.ToCreatedActionResult(this, nameof(GetProduct), new { result.Data?.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, ProductDTO product)
        {
            return (await productService.UpdateProduct(product with { Id = id }))
                .ToActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            return (await productService.DeleteProduct(id))
                .ToActionResult();
        }
    }
}
