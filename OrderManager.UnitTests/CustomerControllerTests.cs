using Microsoft.EntityFrameworkCore;
using Moq;
using OrderManager.API.Controllers;
using OrderManager.API.Database;
using OrderManager.API.Models;
using Shouldly;

namespace OrderManager.UnitTests
{
    public class CustomerControllerTests
    {
        [Fact]
        public async Task GetCustomers_ShouldReturnCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new() { Id = 1, FirstName = "Jan", LastName = "Nowak", Email = "j.nowak@gmail.com" },
                new() { Id = 2, FirstName = "Anastazja", LastName = "Kowalska", Email = "a.kowalska@gmail.com" }
            };
            var queryableCustomer = customers.AsQueryable();
            _mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(queryableCustomer.Provider);
            _mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(queryableCustomer.Expression);
            _mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(queryableCustomer.ElementType);
            _mockCustomersDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(queryableCustomer.GetEnumerator());
            _orderContext.Setup(o => o.Customers).Returns(_mockCustomersDbSet.Object);

            // Act
            var result = await _controller.GetCustomers();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
        }

        private readonly CustomersController _controller;
        private readonly Mock<OrderContext> _orderContext;
        private readonly Mock<DbSet<Customer>> _mockCustomersDbSet;

        public CustomerControllerTests()
        {
            _mockCustomersDbSet = new Mock<DbSet<Customer>>();
            _orderContext = new Mock<OrderContext>();
            _controller = new CustomersController(_orderContext.Object);
        }
    }
}
