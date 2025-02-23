using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Services;
using OrderManager.API.Validations;
using Shouldly;

namespace OrderManager.UnitTests.Services
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task AddProduct_ProdutctNameTooLong_ShouldReturnBadRequestResult()
        {
            // Arrange
            var productName = "productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong";
            var dto = new ProductDTO(0, productName, 100, false);
            var expectedError = ProductErrorMessages.ProductNameTooLong(200, dto.ProductName.Length);

            // Act
            var result = await _productService.AddProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Fact]
        public async Task AddProduct_ProdutctIsNotDigitalAndNullProductStock_ShouldReturnBadRequestResult()
        {
            // Arrange
            var dto = new ProductDTO(0, "productName", 100, false);
            var expectedError = ProductErrorMessages.ProductStockMustBePresent(dto.Id);

            // Act
            var result = await _productService.AddProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Fact]
        public async Task AddProduct_ProdutctIsNotDigitalAndInvalidProductStockQuantity_ShouldReturnBadRequestResult()
        {
            // Arrange
            var dto = new ProductDTO(0, "productName", 100, false, new ProductStockDTO(-1));
            var expectedError = ProductErrorMessages.ProductStockQuantityMustBeGreaterThanZero(dto.Id, dto.ProductStock!.Quantity);

            // Act
            var result = await _productService.AddProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task AddProduct_InvalidProductName_ShouldReturnBadRequestResult(string? productName)
        {
            // Arrange
            var dto = new ProductDTO(0, productName!, 100, false);
            var expectedError = ProductErrorMessages.ProductNameCannotBeEmpty(dto.Id);

            // Act
            var result = await _productService.AddProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AddProduct_InvalidPrice_ShouldReturnBadRequestResult(decimal price)
        {
            // Arrange
            var dto = new ProductDTO(0, string.Empty, price, false);
            var expectedError = ProductErrorMessages.PriceMustBeGreaterThanZero(dto.Id);

            // Act
            var result = await _productService.AddProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Fact]
        public async Task AddProduct_ShouldCreateProduct()
        {
            // Arrange
            var dto = new ProductDTO(0, "New", 100, true);

            // Act
            var result = await _productService.AddProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ProductName.ShouldBe(dto.ProductName);
            result.Data.IsDigital.ShouldBe(dto.IsDigital);
            result.Data.Price.ShouldBe(dto.Price);
            result.Data.ProductStock.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.Created);
            result.ErrorMessage.ShouldBeNull();
            _productRepository.Verify(p => p.Add(It.IsAny<Product>()), Times.Once());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        public async Task AddProduct_WithStock_ShouldCreateProduct(int quantity)
        {
            // Arrange
            var dto = new ProductDTO(0, "New", 100, false, new ProductStockDTO(quantity));

            // Act
            var result = await _productService.AddProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ProductName.ShouldBe(dto.ProductName);
            result.Data.IsDigital.ShouldBe(dto.IsDigital);
            result.Data.Price.ShouldBe(dto.Price);
            result.Data.ProductStock.ShouldNotBeNull();
            result.Data.ProductStock.Quantity.ShouldBe(dto.ProductStock!.Quantity);
            result.StatusCode.ShouldBe(StatusCode.Created);
            result.ErrorMessage.ShouldBeNull();
            _productRepository.Verify(p => p.Add(It.IsAny<Product>()), Times.Once());
        }

        [Fact]
        public async Task UpdateProduct_ProdutctNameTooLong_ShouldReturnBadRequestResult()
        {
            // Arrange
            var productName = "productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong_productNameTooLong";
            var dto = new ProductDTO(0, productName, 100, false);
            var expectedError = ProductErrorMessages.ProductNameTooLong(200, dto.ProductName.Length);

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Fact]
        public async Task UpdateProduct_ProdutctIsNotDigitalAndNullProductStock_ShouldReturnBadRequestResult()
        {
            // Arrange
            var dto = new ProductDTO(0, "productName", 100, false);
            var expectedError = ProductErrorMessages.ProductStockMustBePresent(dto.Id);

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Fact]
        public async Task UpdateProduct_ProdutctIsNotDigitalAndInvalidProductStockQuantity_ShouldReturnBadRequestResult()
        {
            // Arrange
            var dto = new ProductDTO(0, "productName", 100, false, new ProductStockDTO(-1));
            var expectedError = ProductErrorMessages.ProductStockQuantityMustBeGreaterThanZero(dto.Id, dto.ProductStock!.Quantity);

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task UpdateProduct_InvalidProductName_ShouldReturnBadRequestResult(string? productName)
        {
            // Arrange
            var dto = new ProductDTO(0, productName!, 100, false);
            var expectedError = ProductErrorMessages.ProductNameCannotBeEmpty(dto.Id);

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UpdateProduct_InvalidPrice_ShouldReturnBadRequestResult(decimal price)
        {
            // Arrange
            var dto = new ProductDTO(0, string.Empty, price, false);
            var expectedError = ProductErrorMessages.PriceMustBeGreaterThanZero(dto.Id);

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
        }

        [Fact]
        public async Task UpdateProduct_NotFoundProduct_ShouldReturnNotFoundResult()
        {
            // Arrange
            var dto = new ProductDTO(1, "New", 100, true);
            var expectedError = ProductErrorMessages.NotFound(dto.Id);

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _productRepository.Verify(p => p.Update(It.IsAny<Product>()), Times.Never());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        public async Task UpdateProduct_ProductWithStockIsModified_ShouldUpdateProduct(int quantity)
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                IsDigital = false,
                Price = 10,
                ProductName = "Test",
                ProductStock = new ProductStock
                {
                    ProductId = 1,
                    Quantity = 10
                }
            };
            _productRepository.Setup(p => p.GetById(product.Id)).ReturnsAsync(product);
            var dto = new ProductDTO(product.Id, "New", 100, false, new ProductStockDTO(quantity));

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ProductName.ShouldBe(dto.ProductName);
            result.Data.IsDigital.ShouldBe(dto.IsDigital);
            result.Data.Price.ShouldBe(dto.Price);
            result.Data.ProductStock.ShouldNotBeNull();
            result.Data.ProductStock.Quantity.ShouldBe(dto.ProductStock!.Quantity);
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.ErrorMessage.ShouldBeNull();
            _productRepository.Verify(p => p.Update(It.IsAny<Product>()), Times.Once());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        public async Task UpdateProduct_ProductIsDigitalAndIsChangedToProductWithStock_ShouldUpdateProduct(int quantity)
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                IsDigital = false,
                Price = 10,
                ProductName = "Test"
            };
            _productRepository.Setup(p => p.GetById(product.Id)).ReturnsAsync(product);
            var dto = new ProductDTO(product.Id, "New", 100, false, new ProductStockDTO(quantity));

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ProductName.ShouldBe(dto.ProductName);
            result.Data.IsDigital.ShouldBe(dto.IsDigital);
            result.Data.Price.ShouldBe(dto.Price);
            result.Data.ProductStock.ShouldNotBeNull();
            result.Data.ProductStock.Quantity.ShouldBe(dto.ProductStock!.Quantity);
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.ErrorMessage.ShouldBeNull();
            _productRepository.Verify(p => p.Update(It.IsAny<Product>()), Times.Once());
        }

        [Fact]
        public async Task UpdateProduct_ProductHasStockAndIsChangedToDigital_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                IsDigital = false,
                Price = 10,
                ProductName = "Test",
                ProductStock = new ProductStock
                {
                    ProductId = 1,
                    Quantity = 10
                }
            };
            _productRepository.Setup(p => p.GetById(product.Id)).ReturnsAsync(product);
            var dto = new ProductDTO(product.Id, "New", 100, true);

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ProductName.ShouldBe(dto.ProductName);
            result.Data.IsDigital.ShouldBe(dto.IsDigital);
            result.Data.Price.ShouldBe(dto.Price);
            result.Data.ProductStock.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.ErrorMessage.ShouldBeNull();
            _productRepository.Verify(p => p.Update(It.IsAny<Product>()), Times.Once());
        }

        [Fact]
        public async Task UpdateProduct_DigitalProductIsModified_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                IsDigital = false,
                Price = 10,
                ProductName = "Test"
            };
            _productRepository.Setup(p => p.GetById(product.Id)).ReturnsAsync(product);
            var dto = new ProductDTO(product.Id, "New", 100, true);

            // Act
            var result = await _productService.UpdateProduct(dto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ProductName.ShouldBe(dto.ProductName);
            result.Data.IsDigital.ShouldBe(dto.IsDigital);
            result.Data.Price.ShouldBe(dto.Price);
            result.Data.ProductStock.ShouldBeNull();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.ErrorMessage.ShouldBeNull();
            _productRepository.Verify(p => p.Update(It.IsAny<Product>()), Times.Once());
        }

        [Fact]
        public async Task DeleteProduct_ProductNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var id = 1;
            var expectedError = ProductErrorMessages.NotFound(id);

            // Act
            var result = await _productService.DeleteProduct(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _productRepository.Verify(p => p.Delete(It.IsAny<Product>()), Times.Never());
        }

        [Fact]
        public async Task DeleteProduct_ProductIsOrdered_ShouldReturnBadRequestResult()
        {
            // Arrange
            var id = 1;
            var expectedError = ProductErrorMessages.CannotDeleteOrderedProduct(id);
            _productRepository.Setup(p => p.GetById(id)).ReturnsAsync(new Product()
            {
                Id = id,
            });
            _productRepository.Setup(p => p.ProductHasBeenOrdered(id)).ReturnsAsync(true);

            // Act
            var result = await _productService.DeleteProduct(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _productRepository.Verify(p => p.Delete(It.IsAny<Product>()), Times.Never());
        }

        [Fact]
        public async Task DeleteProduct_NotDeleteFromDatabase_ShouldDeleteProduct()
        {
            // Arrange
            var id = 1;
            var expectedError = ProductErrorMessages.NotFound(id);
            _productRepository.Setup(p => p.GetById(id)).ReturnsAsync(new Product()
            {
                Id = id
            });
            _productRepository.Setup(p => p.Delete(It.IsAny<Product>())).ReturnsAsync(false);

            // Act
            var result = await _productService.DeleteProduct(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _productRepository.Verify(p => p.Delete(It.IsAny<Product>()), Times.Once());
        }

        [Fact]
        public async Task DeleteProduct_ShouldDeleteProduct()
        {
            // Arrange
            var id = 1;
            var expectedError = ProductErrorMessages.CannotDeleteOrderedProduct(id);
            _productRepository.Setup(p => p.GetById(id)).ReturnsAsync(new Product()
            {
                Id = id
            });
            _productRepository.Setup(p => p.Delete(It.IsAny<Product>())).ReturnsAsync(true);

            // Act
            var result = await _productService.DeleteProduct(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.NoContent);
            _productRepository.Verify(p => p.Delete(It.IsAny<Product>()), Times.Once());
        }

        private readonly ProductService _productService;
        private readonly Mock<IProductRepository> _productRepository;

        public ProductServiceTests()
        {
            _productRepository = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepository.Object);
        }
    }
}
