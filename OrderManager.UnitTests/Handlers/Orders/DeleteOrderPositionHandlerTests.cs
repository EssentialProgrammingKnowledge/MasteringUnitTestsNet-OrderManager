using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Handlers.Orders;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using Shouldly;
using static OrderManager.API.Handlers.Orders.DeleteOrderPosition;

namespace OrderManager.UnitTests.Handlers.Orders
{
    public class DeleteOrderPositionHandlerTests
    {
        [Fact]
        public async Task Handle_OrderNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var command = new DeleteOrderPosition(1, 1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            _orderRepository.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Handle_OrderCannotBeModified_ShouldReturnBadRequest()
        {
            // Arrange
            var order = new Order { OrderStatus = OrderStatus.Completed };
            var command = new DeleteOrderPosition(1, 1);
            _orderRepository.Setup(r => r.GetDetailsById(command.OrderId)).ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(r => r.Update(order), Times.Never);
        }

        [Fact]
        public async Task Handle_PositionNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var order = new Order { OrderStatus = OrderStatus.New, OrderItems = [] };
            var command = new DeleteOrderPosition(1, 1);
            _orderRepository.Setup(r => r.GetDetailsById(command.OrderId)).ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            _orderRepository.Verify(r => r.Update(order), Times.Never);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var order = new Order { OrderStatus = OrderStatus.New, OrderItems = [new OrderItem { ProductId = 1 }] };
            var command = new DeleteOrderPosition(1, 1);
            _orderRepository.Setup(r => r.GetDetailsById(command.OrderId)).ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(r => r.Update(order), Times.Never);
        }

        [Fact]
        public async Task Handle_SuccessfulRemoval_ShouldReturnOkResult()
        {
            // Arrange
            var order = new Order { OrderStatus = OrderStatus.New, OrderItems = [new OrderItem { ProductId = 1 }, new OrderItem { ProductId = 2 }], Customer = new() };
            var product = new Product() { Id = 1, ProductStock = new ProductStock { ProductId = 1, Quantity = 0 } };
            var command = new DeleteOrderPosition(1, 1);
            _orderRepository.Setup(r => r.GetDetailsById(command.OrderId)).ReturnsAsync(order);
            _productRepository.Setup(p => p.GetById(command.ProductId)).ReturnsAsync(product);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.Data.ShouldNotBeNull();
            result.Data.Positions.ShouldNotBeEmpty();
            result.Data.Positions.Count().ShouldBe(1);
            _orderRepository.Verify(r => r.Update(order), Times.Once);
        }

        private readonly Mock<IOrderRepository> _orderRepository;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly DeleteOrderPositionHandler _handler;

        public DeleteOrderPositionHandlerTests()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _productRepository = new Mock<IProductRepository>();
            _handler = new DeleteOrderPositionHandler(_orderRepository.Object, _productRepository.Object);
        }
    }
}
