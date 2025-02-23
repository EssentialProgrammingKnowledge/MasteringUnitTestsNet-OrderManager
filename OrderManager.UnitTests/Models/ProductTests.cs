using OrderManager.API.Models;
using Shouldly;

namespace OrderManager.UnitTests.Models
{
    public class ProductTests
    {
        [Fact]
        public void HasStock_ProductIsNotDigitalAndHasStock_ShouldReturnTrue()
        {
            // Arrange
            var product = CreateProduct();
            product.IsDigital = false;
            product.ProductStock = new ProductStock { Quantity = 5 };

            // Act
            var result = product.HasStock();

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void HasStock_ProductIsNotDigitalAndHasNoStock_ShouldReturnFalse()
        {
            // Arrange
            var product = CreateProduct();
            product.IsDigital = false;
            product.ProductStock = new ProductStock { Quantity = 0 };

            // Act
            var result = product.HasStock();

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void HasStock_WhenProductIsDigital_ShouldReturnFalse()
        {
            // Arrange
            var product = CreateProduct();
            product.IsDigital = true;

            // Act
            var result = product.HasStock();

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void IncreaseStock_ProductIsNotDigital_ShouldIncreaseQuantity()
        {
            // Arrange
            var product = CreateProduct();
            product.IsDigital = false;
            product.ProductStock = new ProductStock { Quantity = 5 };

            // Act
            product.IncreaseStock(10);

            // Assert
            product.ProductStock.Quantity.ShouldBe(15);
        }

        [Fact]
        public void IncreaseStock_ProductIsDigital_ShouldNotChangeQuantity()
        {
            // Arrange
            var product = CreateProduct();
            product.IsDigital = true;
            product.ProductStock = new ProductStock { Quantity = 5 };

            // Act
            product.IncreaseStock(10);

            // Assert
            product.ProductStock.Quantity.ShouldBe(5);
        }

        [Fact]
        public void DecreaseStock_ProductIsNotDigitalAndHasSufficientStock_ShouldReturnTrue()
        {
            // Arrange
            var product = CreateProduct();
            product.IsDigital = false;
            product.ProductStock = new ProductStock { Quantity = 10 };

            // Act
            var result = product.DecreaseStock(5);

            // Assert
            result.ShouldBeTrue();
            product.ProductStock.Quantity.ShouldBe(5);
        }

        [Fact]
        public void DecreaseStock_ProductIsNotDigitalAndNotEnoughStock_ShouldReturnFalse()
        {
            // Arrange
            var product = CreateProduct();
            product.IsDigital = false;
            product.ProductStock = new ProductStock { Quantity = 3 };

            // Act
            var result = product.DecreaseStock(5);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void DecreaseStock_ProductIsNotDigitalAndStockIsZero_ShouldReturnFalse()
        {
            // Arrange
            var product = CreateProduct();
            product.IsDigital = false;
            product.ProductStock = new ProductStock { Quantity = 0 };

            // Act
            var result = product.DecreaseStock(1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void DecreaseStock_ProductIsDigital_ShouldReturnTrue()
        {
            // Arrange
            var product = CreateProduct();
            product.IsDigital = true;
            product.ProductStock = new ProductStock { Quantity = 0 };

            // Act
            var result = product.DecreaseStock(1);

            // Assert
            result.ShouldBeTrue();
        }

        private Product CreateProduct()
            => new Product
            {
                Id = 1,
                ProductName = "Test Product",
                Price = 100.0m,
                IsDigital = false,
                ProductStock = new ProductStock(),
                OrderItems = new List<OrderItem>()
            };
    }
}
