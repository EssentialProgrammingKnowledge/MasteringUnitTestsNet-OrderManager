using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Handlers.Orders;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using Shouldly;
using static OrderManager.API.Handlers.Orders.UpdateOrderPosition;

namespace OrderManager.UnitTests.Handlers.Orders
{
    public class UpdateOrderPositionHandlerTests
    {
        [Fact]
        public async Task Handle_OrderDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var command = new UpdateOrderPosition(1, new OrderItemDTO(1, 1));

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
            var command = new UpdateOrderPosition(order.Id, new OrderItemDTO(1, 1));
            _orderRepository.Setup(repo => repo.GetDetailsById(command.OrderId)).ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(o => o.Update(order), Times.Never);
        }

        [Fact]
        public async Task Handle_OrderPositionDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var order = new Order { Id = 1, OrderStatus = OrderStatus.New, OrderItems = [] };
            var command = new UpdateOrderPosition(order.Id, new OrderItemDTO(1, 1));
            _orderRepository.Setup(repo => repo.GetDetailsById(command.OrderId)).ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            _orderRepository.Verify(o => o.Update(order), Times.Never);
        }

        [Fact]
        public async Task Handle_ProductDoesNotExist_ShouldReturnBadRequest()
        {
            // Arrange
            var order = new Order { OrderItems = [new() { ProductId = 1 }] };
            var command = new UpdateOrderPosition(order.Id, new OrderItemDTO(1, 1));
            _orderRepository.Setup(repo => repo.GetDetailsById(command.OrderId)).ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(o => o.Update(order), Times.Never);
        }

        [Fact]
        public async Task Handle_DataIsValid_ShouldUpdateOrder()
        {
            // Arrange
            var product = new Product { Id = 1, IsDigital = true };
            var orderItem = new OrderItem { ProductId = product.Id };
            var order = new Order { OrderItems = [orderItem], Customer = new Customer() };
            var command = new UpdateOrderPosition(order.Id, new OrderItemDTO(product.Id, 1));
            _orderRepository.Setup(repo => repo.GetDetailsById(command.OrderId)).ReturnsAsync(order);
            _productRepository.Setup(repo => repo.GetById(command.UpdatePosition.ProductId)).ReturnsAsync(product);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            _orderRepository.Verify(o => o.Update(order), Times.Once);
        }

        private readonly Mock<IOrderRepository> _orderRepository;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly UpdateOrderPositionHandler _handler;

        public UpdateOrderPositionHandlerTests()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _productRepository = new Mock<IProductRepository>();
            _handler = new UpdateOrderPositionHandler(_orderRepository.Object, _productRepository.Object);
        }
    }
}
