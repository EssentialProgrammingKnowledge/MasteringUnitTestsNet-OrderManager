using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Handlers.Orders;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using Shouldly;
using static OrderManager.API.Handlers.Orders.UpdateOrder;

namespace OrderManager.UnitTests.Handlers.Orders
{
    public class UpdateOrderHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var command = new UpdateOrder(new UpdateOrderDto(1, 1, []));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(StatusCode.NotFound, result.StatusCode);
            _orderRepository.Verify(o => o.Update(It.IsAny<Order>()), Times.Never());
        }

        [Fact]
        public async Task Handle_OrderCannotBeModified_ShouldReturnBadRequest()
        {
            // Arrange
            var order = new Order { Id = 1, OrderStatus = OrderStatus.Completed };
            _orderRepository.Setup(repo => repo.GetDetailsById(order.Id)).ReturnsAsync(order);
            var command = new UpdateOrder(new UpdateOrderDto(order.Id, 1, []));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(o => o.Update(order), Times.Never());
        }

        [Fact]
        public async Task Handle_CustomerDoesNotExist_ShouldReturnBadRequest()
        {
            // Arrange
            var order = new Order { Id = 1, OrderStatus = OrderStatus.New };
            _orderRepository.Setup(repo => repo.GetDetailsById(order.Id)).ReturnsAsync(order);
            var command = new UpdateOrder(new UpdateOrderDto(order.Id, 5, []));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            _orderRepository.Verify(o => o.Update(order), Times.Never());
        }

        [Fact]
        public async Task Handle_OrderFound_ShouldReturnSuccesResult()
        {
            var order = new Order { Id = 1, OrderStatus = OrderStatus.New, CustomerId = 3, OrderItems = [new OrderItem { ProductId = 102 }] };
            var customer = new Customer { Id = 5 };
            var products = new List<Product> { new() { Id = 101, IsDigital = true }, new() { Id = 102, IsDigital = true } };
            var command = new UpdateOrder(new UpdateOrderDto(order.Id, 5, [new OrderItemDTO(101, 2)], [102]));
            _orderRepository.Setup(repo => repo.GetDetailsById(order.Id)).ReturnsAsync(order);
            _customerRepository.Setup(repo => repo.GetById(customer.Id)).ReturnsAsync(customer);
            _productRepository.Setup(repo => repo.GetProductsByIds(It.IsAny<List<int>>())).ReturnsAsync(products);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            _orderRepository.Verify(o => o.Update(order), Times.Once());
        }

        private readonly Mock<IOrderRepository> _orderRepository;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<ICustomerRepository> _customerRepository;
        private readonly UpdateOrderHandler _handler;

        public UpdateOrderHandlerTests()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _productRepository = new Mock<IProductRepository>();
            _customerRepository = new Mock<ICustomerRepository>();
            _handler = new UpdateOrderHandler(_orderRepository.Object, _productRepository.Object, _customerRepository.Object);
        }

    }
}
