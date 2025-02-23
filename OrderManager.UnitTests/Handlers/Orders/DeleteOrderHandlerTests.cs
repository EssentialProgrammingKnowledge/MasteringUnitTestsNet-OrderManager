using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Handlers.Orders;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;
using Shouldly;
using static OrderManager.API.Handlers.Orders.DeleteOrder;

namespace OrderManager.UnitTests.Handlers.Orders
{
    public class DeleteOrderHandlerTests
    {
        [Fact]
        public async Task Handle_OrderNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var command = new DeleteOrder(1);
            var expectedError = OrderErrorMessages.NotFound(command.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse(); result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            _orderRepository.Verify(o => o.Delete(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Handle_OrderFoundDeleteNotAffectedAnyRecord_ShouldReturnNotFound()
        {
            // Arrange
            var command = new DeleteOrder(1);
            _orderRepository.Setup(r => r.GetDetailsById(command.Id)).ReturnsAsync(new Order { Id = 1 });
            _orderRepository.Setup(o => o.Delete(It.IsAny<Order>())).ReturnsAsync(false);
            var expectedError = OrderErrorMessages.NotFound(command.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound); result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _orderRepository.Verify(o => o.Delete(It.IsAny<Order>()), Times.Once);
        }

        [Theory]
        [InlineData(OrderStatus.InProgress)]
        [InlineData(OrderStatus.Completed)]
        public async Task Handler_OrderNotInNewStatus_ShouldReturnBadRequest(OrderStatus orderStatus)
        {
            // Arrange
            var id = 1;
            _orderRepository.Setup(o => o.GetDetailsById(id)).ReturnsAsync(new Order
            {
                Id = id,
                OrderStatus = orderStatus
            });
            var command = new DeleteOrder(id);
            var expectedError = OrderErrorMessages.OrderMustBeNewToModify();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
        }

        [Fact]
        public async Task Handle_OrderFound_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new DeleteOrder(1);
            _orderRepository.Setup(r => r.GetDetailsById(command.Id)).ReturnsAsync(new Order { Id = 1 });
            _orderRepository.Setup(o => o.Delete(It.IsAny<Order>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.NoContent);
            _orderRepository.Verify(o => o.Delete(It.IsAny<Order>()), Times.Once);
        }

        private readonly Mock<IOrderRepository> _orderRepository = new();
        private readonly DeleteOrderHandler _handler;

        public DeleteOrderHandlerTests()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _handler = new DeleteOrderHandler(_orderRepository.Object);
        }
    }
}
