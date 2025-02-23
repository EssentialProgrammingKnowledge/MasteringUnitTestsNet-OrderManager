using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Handlers.Orders;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;
using Shouldly;
using static OrderManager.API.Handlers.Orders.AddOrder;

namespace OrderManager.UnitTests.Handlers.Orders
{
    public class AddOrderHandlerTests
    {
        [Fact]
        public async Task Handle_OrderDtoWithoutPositions_ShouldReturnBadRequestResult()
        {
            // Arrange
            var expectedError = OrderErrorMessages.OrderMustContainAtLeastOneItem();
            var command = new AddOrder(new AddOrderDto(1, []));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.Data.ShouldBeNull();
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldBeNull();
            _orderRepository.Verify(o => o.Add(It.IsAny<Order>()), Times.Never());
        }

        [Fact]
        public async Task Handle_OrderDtoWithInvalidPositionsQuantity_ShouldReturnBadRequestResult()
        {
            // Arrange
            var invalidQuantityPositions = new List<OrderItemDTO>
            {
                new(1, -2),
                new(2, 0),
            };
            var expectedError = OrderErrorMessages.PositionsQuantityMustBeGreaterThanZero(invalidQuantityPositions);
            var command = new AddOrder(new AddOrderDto(1, invalidQuantityPositions));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.Data.ShouldBeNull();
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _orderRepository.Verify(o => o.Add(It.IsAny<Order>()), Times.Never());
        }

        [Fact]
        public async Task Handle_CustomerNotFound_ShouldReturnBadRequestResult()
        {
            // Arrange
            var customerId = 999;
            var expectedError = CustomerErrorMessages.NotFound(customerId);
            var command = new AddOrder(new AddOrderDto(customerId, [new(1, 2)]));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.Data.ShouldBeNull();
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _orderRepository.Verify(o => o.Add(It.IsAny<Order>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ProductsNotFound_ShouldReturnBadRequestResult()
        {
            // Arrange
            var command = new AddOrder(new AddOrderDto(1, [new(1, 2)]));
            var expectedError = OrderErrorMessages.PositionsNotFound(0, command.OrderDto.Positions.Select(p => p.ProductId).ToList());
            _customerRepository.Setup(c => c.GetById(It.IsAny<int>())).ReturnsAsync(new Customer { Id = 1 });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.Data.ShouldBeNull();
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _orderRepository.Verify(o => o.Add(It.IsAny<Order>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ProductNotAvailable_ShouldReturnBadRequestResult()
        {
            // Arrange
            var customer = new Customer { Id = 1, FirstName = "Test", LastName = "Customer" };
            var product = new Product { Id = 1, Price = 100m, ProductStock = new ProductStock { Quantity = 10 } };
            var command = new AddOrder(new AddOrderDto(customer.Id, [new(product.Id, 1000)]));
            _customerRepository.Setup(c => c.GetById(customer.Id)).ReturnsAsync(customer);
            _productRepository.Setup(p => p.GetProductsByIds(It.IsAny<List<int>>())).ReturnsAsync(new List<Product> { product });
            var expectedError = ProductErrorMessages.ProductsNotAvailable(command.OrderDto.Positions.Select(p => p.ProductId));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.Data.ShouldBeNull();
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _orderRepository.Verify(o => o.Add(It.IsAny<Order>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ValidOrder_ShouldReturnCreatedResult()
        {
            // Arrange
            var customer = new Customer { Id = 1, FirstName = "Test", LastName = "Customer", Email = "email@email.com" };
            var product = new Product { Id = 1, Price = 100m, ProductStock = new ProductStock { Quantity = 10 } };
            var command = new AddOrder(new AddOrderDto(customer.Id, [new(product.Id, 2)]));
            _customerRepository.Setup(repo => repo.GetById(It.IsAny<int>())).ReturnsAsync(customer);
            _productRepository.Setup(repo => repo.GetProductsByIds(It.IsAny<List<int>>())).ReturnsAsync(new List<Product> { product });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.OrderNumber.ShouldNotBeNullOrEmpty();
            result.Data.Customer.ShouldNotBeNull();
            result.Data.Customer.Id.ShouldBe(customer.Id);
            result.Data.Customer.FirstName.ShouldBe(customer.FirstName);
            result.Data.Customer.LastName.ShouldBe(customer.LastName);
            result.Data.Customer.Email.ShouldBe(customer.Email);
            result.Data.TotalPrice.ShouldBe(product.Price * 2);
            result.Data.OrderStatus.ShouldBe(OrderStatus.New);
            result.Data.Positions.ShouldContain(p => p.ProductId == product.Id);
            _orderRepository.Verify(o => o.Add(It.IsAny<Order>()), Times.Once());
        }

        private readonly AddOrderHandler _handler;
        private readonly Mock<IOrderRepository> _orderRepository;
        private readonly Mock<IProductRepository> _productRepository;
        private readonly Mock<ICustomerRepository> _customerRepository;

        public AddOrderHandlerTests()
        {
            _orderRepository = new Mock<IOrderRepository>();
            _productRepository = new Mock<IProductRepository>();
            _customerRepository = new Mock<ICustomerRepository>();
            _handler = new AddOrderHandler(_orderRepository.Object, _productRepository.Object, _customerRepository.Object);
        }
    }
}
