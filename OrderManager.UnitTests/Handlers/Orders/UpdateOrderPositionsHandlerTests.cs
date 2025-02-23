using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Handlers.Orders;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using Shouldly;
using static OrderManager.API.Handlers.Orders.UpdateOrderPositions;

namespace OrderManager.UnitTests.Handlers.Orders
{
    public class UpdateOrderPositionsHandlerTests
    {

        [Fact]
        public async Task Handle_UpdatePositionsAreEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            var command = new UpdateOrderPositions(1, []);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(o => o.Update(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Handle_OrderDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _orderRepository.Setup(repo => repo.GetDetailsById(It.IsAny<int>())).ReturnsAsync((Order)null);
            var command = new UpdateOrderPositions(1, [new(1, 2)]);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            _orderRepository.Verify(o => o.Update(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Handle_OrderCannotBeModified_ShouldReturnBadRequest()
        {
            // Arrange
            var order = new Order { Id = 1, OrderStatus = OrderStatus.Completed };
            _orderRepository.Setup(repo => repo.GetDetailsById(It.IsAny<int>())).ReturnsAsync(order);
            var command = new UpdateOrderPositions(1, [new(1, 2)]);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(o => o.Update(order), Times.Never);
        }

        [Fact]
        public async Task Handle_PositionsAreMissingInOrder_ShouldReturnBadRequest()
        {
            // Arrange
            var order = new Order { Id = 1, OrderItems = [new() { ProductId = 2 }] };
            _orderRepository.Setup(repo => repo.GetDetailsById(It.IsAny<int>())).ReturnsAsync(order);
            var command = new UpdateOrderPositions(1, [new(1, 2)]);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(o => o.Update(order), Times.Never);
        }

        [Fact]
        public async Task Handle_ProductsNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var order = new Order { Id = 1, OrderItems = [new() { ProductId = 1 }] };
            _orderRepository.Setup(repo => repo.GetDetailsById(It.IsAny<int>())).ReturnsAsync(order);
            _productRepository.Setup(repo => repo.GetProductsByIds(It.IsAny<List<int>>())).ReturnsAsync([]);
            var command = new UpdateOrderPositions(1, [new(1, 2)]);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(o => o.Update(order), Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateIsSuccessful_ShouldReturnSuccessResult()
        {
            var order = new Order { Id = 1, OrderItems = [new() { ProductId = 1 }], Customer = new() };
            var product = new Product { Id = 1, IsDigital = true };
            _orderRepository.Setup(repo => repo.GetDetailsById(It.IsAny<int>())).ReturnsAsync(order);
            _productRepository.Setup(repo => repo.GetProductsByIds(It.IsAny<List<int>>())).ReturnsAsync([product]);
            var command = new UpdateOrderPositions(1, [new(1, 2)]);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            _orderRepository.Verify(o => o.Update(order), Times.Once);
        }

        private readonly Mock<IOrderRepository> _orderRepository = new();
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly UpdateOrderPositionsHandler _handler;

        public UpdateOrderPositionsHandlerTests()
        {
            _handler = new UpdateOrderPositionsHandler(_orderRepository.Object, _productRepository.Object);
        }
    }
}
