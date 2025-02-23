using OrderManager.API.DTO;
using OrderManager.API.Models;
using OrderManager.API.Validations;
using Shouldly;

namespace OrderManager.UnitTests.Models
{
    public class OrderTests
    {
        [Fact]
        public void AddPositions_ProductNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var positions = new List<OrderItemDTO>
            {
                new (1, 2)
            };
            var productsDict = new Dictionary<int, Product>();

            // Act
            var result = order.AddPositions(positions, productsDict);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = OrderErrorMessages.PositionsNotFound(order.Id, [1]);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);
        }

        [Fact]
        public void AddPositions_ProductNotAvailable_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 0 } };
            var positions = new List<OrderItemDTO>
            {
                new (1, 2)
            };
            var productsDict = new Dictionary<int, Product> { { 1, product } };

            // Act
            var result = order.AddPositions(positions, productsDict);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = ProductErrorMessages.ProductsNotAvailable([1]);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);
        }

        [Fact]
        public void AddPositions_Success_ShouldReturnSuccessResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 10 } };
            var positions = new List<OrderItemDTO>
            {
                new (1, 2)
            };
            var productsDict = new Dictionary<int, Product> { { 1, product } };

            // Act
            var result = order.AddPositions(positions, productsDict);

            // Assert
            result.Success.ShouldBeTrue();
            order.OrderItems.Count.ShouldBe(1);
            order.OrderItems[0].Quantity.ShouldBe(2);
            order.TotalPrice.ShouldBe(200M);
        }

        [Fact]
        public void RemovePositions_ProductNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var positionsIds = new List<int> { 1 };
            var productsDict = new Dictionary<int, Product>();

            // Act
            var result = order.RemovePositions(positionsIds, productsDict);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = OrderErrorMessages.PositionsNotFound(order.Id, [1]);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);
        }

        [Fact]
        public void RemovePositions_Success_ShouldReturnSuccessResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 10 } };
            var positions = new List<OrderItemDTO>
            {
                new (1, 2)
            };
            order.AddPositions(positions, new Dictionary<int, Product> { { 1, product } });
            var positionsIds = new List<int> { 1 };

            var productsDict = new Dictionary<int, Product> { { 1, product } };

            // Act
            var result = order.RemovePositions(positionsIds, productsDict);

            // Assert
            result.Success.ShouldBeTrue();
            order.OrderItems.Count.ShouldBe(0);
            order.TotalPrice.ShouldBe(0m);
        }

        [Fact]
        public void ModifyPositions_ProductNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var positions = new List<OrderItemDTO>
            {
                new (1, 2)
            };
            var productsDict = new Dictionary<int, Product>();

            // Act
            var result = order.ModifyPostions(positions, productsDict);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = OrderErrorMessages.PositionsNotFound(order.Id, [1]);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);
        }

        [Fact]
        public void ModifyPositions_ProductNotAvailable_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 2 } };
            var positions = new List<OrderItemDTO>
            {
                new (1, 2)
            };
            var productsDict = new Dictionary<int, Product> { { 1, product } };
            order.AddPositions(positions, productsDict);
            var positionsToModify = new List<OrderItemDTO>
            {
                new (1, 3)
            };

            // Act
            var result = order.ModifyPostions(positionsToModify, productsDict);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = ProductErrorMessages.ProductsNotAvailable([1]);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);
        }

        [Fact]
        public void ModifyPositions_Success_ShouldReturnSuccessResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 10 } };
            var positions = new List<OrderItemDTO>
            {
                new (1, 2)
            };
            var productsDict = new Dictionary<int, Product> { { 1, product } };
            order.AddPositions(positions, productsDict);
            var positionsToModified = new List<OrderItemDTO>
            {
                new (1, 3)
            };

            // Act
            var result = order.ModifyPostions(positionsToModified, productsDict);

            // Assert
            result.Success.ShouldBeTrue();
            order.OrderItems[0].Quantity.ShouldBe(3);
            order.TotalPrice.ShouldBe(300m);
        }

        [Fact]
        public void AddPosition_ProductNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var position = new OrderItemDTO(1, 2);

            // Act
            var result = order.AddPosition(position, null!);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = OrderErrorMessages.PositionNotFound(position.ProductId, position.ProductId);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);
        }

        [Fact]
        public void AddPosition_ProductNotAvailable_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 0 } };
            var position = new OrderItemDTO(1, 2);

            // Act
            var result = order.AddPosition(position, product);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = ProductErrorMessages.ProductNotAvailable(product.Id);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);
        }

        [Fact]
        public void AddPosition_Success_ShouldReturnSuccessResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 10 } };
            var position = new OrderItemDTO(1, 2);

            // Act
            var result = order.AddPosition(position, product);

            // Assert
            result.Success.ShouldBeTrue();
            order.OrderItems.Count.ShouldBe(1);
            order.OrderItems[0].Quantity.ShouldBe(2);
            order.TotalPrice.ShouldBe(200m);
        }

        [Fact]
        public void RemovePosition_ProductNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 10 } };
            var position = new OrderItemDTO(1, 2);
            order.AddPosition(position, product);
            var removeProductId = 2;

            // Act
            var result = order.RemovePosition(removeProductId, product);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = OrderErrorMessages.PositionNotFound(order.Id, removeProductId);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);
        }

        [Fact]
        public void RemovePosition_Success_ShouldReturnSuccessResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 10 } };
            var position = new OrderItemDTO(1, 2);
            order.AddPosition(position, product);

            // Act
            var result = order.RemovePosition(1, product);

            // Assert
            result.Success.ShouldBeTrue();
            order.OrderItems.Count.ShouldBe(0);
            order.TotalPrice.ShouldBe(0m);
        }

        [Fact]
        public void ModifyPosition_ProductNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 10 } };
            var position = new OrderItemDTO(1, 2);

            // Act
            var result = order.ModifyPosition(position, null!);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = OrderErrorMessages.PositionNotFound(order.Id, position.ProductId);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);

        }

        [Fact]
        public void ModifyPosition_ProductNotAvailable_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 2 } };
            var position = new OrderItemDTO(1, 2);
            order.AddPosition(position, product);
            var modifiedPosition = new OrderItemDTO(1, 3);

            // Act
            var result = order.ModifyPosition(modifiedPosition, product);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = ProductErrorMessages.ProductNotAvailable(product.Id);
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters!.Values);
        }

        [Fact]
        public void ModifyPosition_Success_ShouldReturnSuccessResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 10 } };
            var position = new OrderItemDTO(1, 2);
            var productsDict = new Dictionary<int, Product> { { 1, product } };
            order.AddPosition(position, product);
            var modifiedPosition = new OrderItemDTO(1, 5);

            // Act
            var result = order.ModifyPosition(modifiedPosition, product);

            // Assert
            result.Success.ShouldBeTrue();
            order.OrderItems[0].Quantity.ShouldBe(5);
            order.TotalPrice.ShouldBe(500m);
        }
        [Fact]
        public void AddPosition_InvalidPosition_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product();

            // Act
            var result = order.AddPosition(null!, product);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = OrderErrorMessages.InvalidPosition();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
        }

        [Fact]
        public void RemovePosition_InvalidPosition_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var invalidProductId = 1;

            var result = order.RemovePosition(invalidProductId, null!);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = OrderErrorMessages.InvalidPosition();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
        }

        [Fact]
        public void ModifyPosition_InvalidPosition_ShouldReturnFailureResult()
        {
            // Arrange
            var order = CreateOrder();
            var product = new Product { Id = 1, Price = 100m, IsDigital = false, ProductStock = new ProductStock { Quantity = 10 } };

            var result = order.ModifyPosition(null!, product);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeNull();
            var expectedError = OrderErrorMessages.InvalidPosition();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
        }

        private Order CreateOrder()
            => new()
            {
                Id = 1,
                OrderNumber = "ORD123",
                OrderStatus = OrderStatus.New,
                CreatedAt = DateTime.Now,
                CustomerId = 1,
                Customer = new Customer { Id = 1, FirstName = "Customer Name" }
            };
    }
}
