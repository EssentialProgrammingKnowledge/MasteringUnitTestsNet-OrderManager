using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Services
{
    internal sealed class ProductService
        (
            IProductRepository productRepository
        )
        : IProductService
    {
        public async Task<Result<ProductDTO>> AddProduct(ProductDTO productDto)
        {
            var result = ValidateProduct(productDto);
            if (!result.Success)
            {
                return Result<ProductDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var product = new Product
            {
                ProductName = productDto.ProductName,
                Price = productDto.Price,
                IsDigital = productDto.IsDigital,
            };
            if (!productDto.IsDigital)
            {
                product.ProductStock ??= new ProductStock
                {
                    Quantity = productDto.ProductStock?.Quantity ?? 0
                };
            }
            await productRepository.Add(product);
            return Result<ProductDTO>.CreatedResult(product.AsDto());
        }

        public async Task<Result> DeleteProduct(int id)
        {
            var product = await productRepository.GetById(id);
            if (product is null)
            {
                return Result.NotFoundResult(ProductErrorMessages.NotFound(id));
            }

            if (await productRepository.ProductHasBeenOrdered(id))
            {
                return Result.BadRequestResult(ProductErrorMessages.CannotDeleteOrderedProduct(id));
            }

            var result = await productRepository.Delete(product);
            return result ?
                Result.NoContentResult()
                : Result.NotFoundResult(ProductErrorMessages.NotFound(id));
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProducts()
        {
            return (await productRepository.GetAll())
                                           .Select(e => e.AsDto())
                                           .ToList();
        }

        public async Task<Result<ProductDTO>> GetProductById(int id)
        {
            var product = await productRepository.GetById(id);
            if (product is null)
            {
                return Result<ProductDTO>.NotFoundResult(ProductErrorMessages.NotFound(id));
            }

            return Result<ProductDTO>.OkResult(product.AsDto());
        }

        public async Task<Result<ProductDTO>> UpdateProduct(ProductDTO productDto)
        {
            var result = ValidateProduct(productDto);
            if (!result.Success)
            {
                return Result<ProductDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var product = await productRepository.GetById(productDto.Id);
            if (product is null)
            {
                return Result<ProductDTO>.NotFoundResult(ProductErrorMessages.NotFound(productDto.Id));
            }

            product.ProductName = productDto.ProductName;
            product.Price = productDto.Price;
            product.IsDigital = productDto.IsDigital;
            if (productDto.IsDigital)
            {
                product.ProductStock = null;
            }
            else
            {
                product.ProductStock ??= new ProductStock
                {
                    Quantity = 0
                };
                product.ProductStock.Quantity = productDto.ProductStock?.Quantity ?? 0;
            }

            await productRepository.Update(product);
            return Result<ProductDTO>.OkResult(product.AsDto());
        }


        private ValidationResult ValidateProduct(ProductDTO dto)
        {
            if (dto.Price <= 0)
            {
                return ValidationResult.FailureResult(ProductErrorMessages.PriceMustBeGreaterThanZero(dto.Id));
            }

            if (string.IsNullOrWhiteSpace(dto.ProductName))
            {
                return ValidationResult.FailureResult(ProductErrorMessages.ProductNameCannotBeEmpty(dto.Id));
            }

            if (dto.ProductName.Length > 200)
            {
                return ValidationResult.FailureResult(ProductErrorMessages.ProductNameTooLong(200, dto.ProductName.Length));
            }

            if (dto.IsDigital)
            {
                return ValidationResult.SuccessResult();
            }

            if (dto.ProductStock is null)
            {
                return ValidationResult.FailureResult(ProductErrorMessages.ProductStockMustBePresent(dto.Id));
            }

            if (dto.ProductStock.Quantity <= 0)
            {
                return ValidationResult.FailureResult(ProductErrorMessages.ProductStockQuantityMustBeGreaterThanZero(dto.Id, dto.ProductStock.Quantity));
            }

            return ValidationResult.SuccessResult();
        }
    }
}
