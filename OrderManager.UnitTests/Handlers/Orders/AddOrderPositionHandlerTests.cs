using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Handlers.Orders;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;
using Shouldly;
using static OrderManager.API.Handlers.Orders.AddOrderPosition;

namespace OrderManager.UnitTests.Handlers.Orders
{
    public class AddOrderPositionHandlerTests
    {
        [Fact]
        public async Task Handle_OrderNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var command = new AddOrderPosition(1, new OrderItemDTO(1, 2));
            var expectedError = OrderErrorMessages.NotFound(command.OrderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _orderRepository.Verify(o => o.Update(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidPosition_ShouldReturnBadRequestResult()
        {
            // Arrange
            var handler = new AddOrderPositionHandler(_orderRepository.Object, _productRepository.Object);
            var command = new AddOrderPosition(1, new OrderItemDTO(1, 0));
            var expectedError = OrderErrorMessages.PositionQuantityMustBeGreaterThanZero(command.NewPosition.ProductId, command.NewPosition.Quantity);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _orderRepository.Verify(o => o.Update(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidOrder_ShouldReturnCreatedResult()
        {
            // Arrange
            var order = new Order { Id = 1, OrderStatus = OrderStatus.New, Customer = new Customer() { Id = 1, FirstName = nameof(Customer.FirstName), LastName = nameof(Customer.LastName), Email = nameof(Customer.Email) } };
            var product = new Product { Id = 1, IsDigital = true };
            var handler = new AddOrderPositionHandler(_orderRepository.Object, _productRepository.Object);
            var command = new AddOrderPosition(1, new OrderItemDTO(1, 2));
            _orderRepository.Setup(r => r.GetDetailsById(command.OrderId)).ReturnsAsync(order);
            _productRepository.Setup(p => p.GetById(command.NewPosition.ProductId)).ReturnsAsync(product);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.Created);
            result.Data.ShouldNotBeNull();
            result.Data.Positions.ShouldNotBeNull().ShouldNotBeEmpty();
            result.Data.Positions.ShouldContain(p => p.ProductId == command.NewPosition.ProductId);
            _orderRepository.Verify(o => o.Update(It.IsAny<Order>()), Times.Once);
        }

        private readonly Mock<IOrderRepository> _orderRepository = new();
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly AddOrderPositionHandler _handler;

        public AddOrderPositionHandlerTests()
        {
            _handler = new AddOrderPositionHandler(_orderRepository.Object, _productRepository.Object);
        }
    }
}
