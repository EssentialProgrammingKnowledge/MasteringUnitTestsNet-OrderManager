using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Handlers.Orders;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;
using Shouldly;
using static OrderManager.API.Handlers.Orders.ChangeOrderStatus;

namespace OrderManager.UnitTests.Handlers.Orders
{
    public class ChangeOrderStatusHandlerTests
    {
        [Fact]
        public async Task Handle_OrderNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var command = new ChangeOrderStatus(new ChangeOrderStatusDTO(1, OrderStatus.InProgress));
            var expectedError = OrderErrorMessages.NotFound(command.Dto.Id);

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
            _orderRepository.Verify(o => o.Update(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Handle_OrderFound_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new ChangeOrderStatus(new ChangeOrderStatusDTO(1, OrderStatus.InProgress));
            _orderRepository.Setup(r => r.GetById(command.Dto.Id)).ReturnsAsync(new Order { Id = 1, OrderStatus = OrderStatus.New });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.OrderStatus.ShouldBe(command.Dto.OrderStatus);
            result.StatusCode.ShouldBe(StatusCode.Ok);
            _orderRepository.Verify(o => o.Update(It.IsAny<Order>()), Times.Once);
        }

        private readonly Mock<IOrderRepository> _orderRepository = new();
        private readonly ChangeOrderStatusHandler _handler;

        public ChangeOrderStatusHandlerTests()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _handler = new ChangeOrderStatusHandler(_orderRepository.Object);
        }
    }
}
