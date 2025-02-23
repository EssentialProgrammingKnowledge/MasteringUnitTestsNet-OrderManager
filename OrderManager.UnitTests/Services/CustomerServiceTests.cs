using Moq;
using OrderManager.API.DTO;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Services;
using OrderManager.API.Validations;
using Shouldly;

namespace OrderManager.UnitTests.Services
{
    public class CustomerServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task AddCustomer_InvalidFirstName_ShouldReturnBadRequest(string? firstName)
        {
            // Arrange
            var customer = new CustomerDTO(0, firstName!, "lastName", "email@email.com");
            var expectedError = CustomerErrorMessages.FirstNameCannotBeEmpty(customer.Id);

            // Act
            var result = await _customerService.AddCustomer(customer);

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
            _customerRepository.Verify(c => c.Add(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task AddCustomer_TooLongFirstName_ShouldReturnBadRequest()
        {
            // Arrange
            var customer = new CustomerDTO(0, "FirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstName12", "lastName", "email@email.com");
            var expectedError = CustomerErrorMessages.FirstNameTooLong(100, customer.FirstName.Length);
            
            // Act
            var result = await _customerService.AddCustomer(customer);

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
            _customerRepository.Verify(c => c.Add(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task AddCustomer_InvalidLastName_ShouldReturnBadRequest(string? lastName)
        {
            // Arrange
            var customer = new CustomerDTO(0, "FirstName", lastName!, "email@email.com");
            var expectedError = CustomerErrorMessages.LastNameCannotBeEmpty(customer.Id);
            
            // Act
            var result = await _customerService.AddCustomer(customer);

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
            _customerRepository.Verify(c => c.Add(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task AddCustomer_TooLongLastName_ShouldReturnBadRequest()
        {
            // Arrange
            var customer = new CustomerDTO(0, "FirstName", "LastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastName12345", "email@email.com");
            var expectedError = CustomerErrorMessages.LastNameTooLong(100, customer.LastName.Length);
            
            // Act
            var result = await _customerService.AddCustomer(customer);

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
            _customerRepository.Verify(c => c.Add(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData("email")]
        [InlineData("email@")]
        [InlineData("email@.")]
        [InlineData("email@aa.")]
        [InlineData("email@aa.aa.")]
        [InlineData("testemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytestemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytestemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytest1234567890K@aa.aa.aaaaaaaaaaaaaaaaaaaaaaaabcdefg")]
        public async Task AddCustomer_InvalidEmail_ShouldReturnBadRequest(string? email)
        {
            // Arrange
            var customer = new CustomerDTO(0, "firstName", "lastName", email!);
            var expectedError = CustomerErrorMessages.InvalidEmail(customer.Email);

            // Act
            var result = await _customerService.AddCustomer(customer);

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
            _customerRepository.Verify(c => c.Add(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("FirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstName1", "lastName", "email@email.com")]
        [InlineData("FirstName", "LastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastName123", "email@email.com")]
        [InlineData("FirstName", "LastName", "testemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytestemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytestemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytest1234567890K@aa.aa.aaaaaaaaaaaaaaaaaaaaaaaabcdef")]
        public async Task AddCustomer_ValidFirstNameLastNameAndEmail_ShouldReturnSuccessResult(string firstName, string lastName, string email)
        {
            // Arrange Act
            var result = await _customerService.AddCustomer(new CustomerDTO(0, firstName, lastName, email));

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.Created);
            result.Data.ShouldNotBeNull();
            result.ErrorMessage.ShouldBeNull();
            _customerRepository.Verify(c => c.Add(It.IsAny<Customer>()), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task UpdateCustomer_InvalidFirstName_ShouldReturnBadRequest(string? firstName)
        {
            // Arrange
            var customer = new CustomerDTO(1, firstName!, "lastName", "email@email.com");
            var expectedError = CustomerErrorMessages.FirstNameCannotBeEmpty(customer.Id);
            
            // Act
            var result = await _customerService.UpdateCustomer(customer);

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
            _customerRepository.Verify(c => c.Update(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateCustomer_TooLongFirstName_ShouldReturnBadRequest()
        {
            // Arrange
            var customer = new CustomerDTO(1, "FirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstName12", "lastName", "email@email.com");
            var expectedError = CustomerErrorMessages.FirstNameTooLong(100, customer.FirstName.Length);
            
            // Act
            var result = await _customerService.UpdateCustomer(customer);

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
            _customerRepository.Verify(c => c.Update(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task UpdateCustomer_InvalidLastName_ShouldReturnBadRequest(string? lastName)
        {
            // Arrange
            var customer = new CustomerDTO(1, "FirstName", lastName!, "email@email.com");
            var expectedError = CustomerErrorMessages.LastNameCannotBeEmpty(customer.Id);
            
            // Act
            var result = await _customerService.UpdateCustomer(customer);

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
            _customerRepository.Verify(c => c.Update(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }
        
        [Fact]
        public async Task UpdateCustomer_TooLongLastName_ShouldReturnBadRequest()
        {
            // Arrange
            var customer = new CustomerDTO(1, "FirstName", "LastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastName12345", "email@email.com");
            var expectedError = CustomerErrorMessages.LastNameTooLong(100, customer.LastName.Length);
            
            // Act
            var result = await _customerService.UpdateCustomer(customer);

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
            _customerRepository.Verify(c => c.Update(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData("email")]
        [InlineData("email@")]
        [InlineData("email@.")]
        [InlineData("email@aa.")]
        [InlineData("email@aa.aa.")]
        [InlineData("testemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytestemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytestemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytest1234567890K@aa.aa.aaaaaaaaaaaaaaaaaaaaaaaabcdefg")]
        public async Task UpdateCustomer_InvalidEmail_ShouldReturnBadRequest(string? email)
        {
            // Arrange
            var customer = new CustomerDTO(1, "firstName", "lastName", email!);
            var expectedError = CustomerErrorMessages.InvalidEmail(customer.Email);
            
            // Act
            var result = await _customerService.UpdateCustomer(customer);

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
            _customerRepository.Verify(c => c.Update(It.IsAny<Customer>()), Times.Never());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task UpdateCustomer_CustomerNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var id = 1;
            var expectedError = CustomerErrorMessages.NotFound(id);

            // Act
            var result = await _customerService.UpdateCustomer(new CustomerDTO(id, "firstName", "lastName", "email@email.com"));

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.Data.ShouldBeNull();
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _customerRepository.Verify(c => c.Update(It.IsAny<Customer>()), Times.Never());
            _customerRepository.Verify(c => c.GetById(id), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("FirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstNameFirstName1", "lastName", "email@email.com")]
        [InlineData("FirstName", "LastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastNameLastName123", "email@email.com")]
        [InlineData("FirstName", "LastName", "testemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytestemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytestemail1234123467890456456345emailaaaaaaaaaaatest1234567890qweretytest1234567890@aa.aa.aaaaaaaaaaaaaaaaaaaaaaaa")]
        public async Task UpdateCustomer_ValidFirstNameLastNameAndEmail_ShouldReturnSuccessResult(string firstName, string lastName, string email)
        {
            // Arrange
            var id = 1;
            var customer = new Customer
            {
                Id = id,
                FirstName = "firstName",
                LastName = "lastName",
                Email = "email@email"
            };
            _customerRepository.Setup(c => c.GetById(id)).ReturnsAsync(customer);

            // Act
            var result = await _customerService.UpdateCustomer(new CustomerDTO(id, firstName, lastName, email));

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.Data.ShouldNotBeNull();
            result.ErrorMessage.ShouldBeNull();
            _customerRepository.Verify(c => c.Update(customer), Times.Once());
            _customerRepository.Verify(c => c.GetById(id), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteCustomer_CustomerNotFound_ShouldReturnReturnNotFound()
        {
            // Arrange
            var id = 1;
            var expectedError = CustomerErrorMessages.NotFound(id);

            // Act
            var result = await _customerService.DeleteCustomer(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _customerRepository.Verify(c => c.GetById(id), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteCustomer_CustomerAssignedToOrder_ShouldReturnBadRequest()
        {
            // Arrange
            var id = 1;
            var customer = new Customer
            {
                Id = id,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "email@email.com",
            };
            _customerRepository.Setup(c => c.GetById(id)).ReturnsAsync(customer);
            _customerRepository.Setup(c => c.HasAnyOrder(id)).ReturnsAsync(true);
            var expectedError = CustomerErrorMessages.CannotDeleteCustomerWithOrders(id);

            // Act
            var result = await _customerService.DeleteCustomer(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.BadRequest);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _customerRepository.Verify(c => c.GetById(id), Times.Once());
            _customerRepository.Verify(c => c.HasAnyOrder(id), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteCustomer_CustomerFoundAndNotAssignedToAnyOrderAndNotDeleteFromDatabase_ShouldReturnNotFound()
        {
            // Arrange
            var id = 1;
            _customerRepository.Setup(c => c.GetById(id)).ReturnsAsync(new Customer
            {
                Id = id,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "email@email.com",
            });
            _customerRepository.Setup(c => c.HasAnyOrder(id)).ReturnsAsync(false);
            _customerRepository.Setup(c => c.Delete(It.IsAny<Customer>())).ReturnsAsync(false);
            var expectedError = CustomerErrorMessages.NotFound(id);

            // Act
            var result = await _customerService.DeleteCustomer(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _customerRepository.Verify(c => c.GetById(id), Times.Once());
            _customerRepository.Verify(c => c.HasAnyOrder(id), Times.Once());
            _customerRepository.Verify(c => c.Delete(It.IsAny<Customer>()), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeleteCustomer_CustomerFoundAndNotAssignedToAnyOrder_ShouldReturnSuccessResult()
        {
            // Arrange
            var id = 1;
            _customerRepository.Setup(c => c.GetById(id)).ReturnsAsync(new Customer
            {
                Id = id,
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "email@email.com",
            });
            _customerRepository.Setup(c => c.HasAnyOrder(id)).ReturnsAsync(false);
            _customerRepository.Setup(c => c.Delete(It.IsAny<Customer>())).ReturnsAsync(true);

            // Act
            var result = await _customerService.DeleteCustomer(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.NoContent);
            result.ErrorMessage.ShouldBeNull();
            _customerRepository.Verify(c => c.GetById(id), Times.Once());
            _customerRepository.Verify(c => c.HasAnyOrder(id), Times.Once());
            _customerRepository.Verify(c => c.Delete(It.IsAny<Customer>()), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAllCustomers_ShouldReturnSuccessResult()
        {
            // Arrange Act
            var result = await _customerService.GetAllCustomers();

            // Assert
            result.ShouldNotBeNull();
            _customerRepository.Verify(c => c.GetAll(), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetCustomerById_ShouldReturnSuccessResult()
        {
            // Arrange
            var id = 1;
            _customerRepository.Setup(c => c.GetById(id)).ReturnsAsync(new Customer
            {
                Id = id,
                FirstName = "firstName",
                LastName = "lastName",
                Email = "email@email.com",
            });

            // Act
            var result = await _customerService.GetCustomerById(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.StatusCode.ShouldBe(StatusCode.Ok);
            result.Data.ShouldNotBeNull();
            result.ErrorMessage.ShouldBeNull();
            _customerRepository.Verify(c => c.GetById(id), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetCustomerById_CustomerNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var id = 1;
            var expectedError = CustomerErrorMessages.NotFound(id);

            // Act
            var result = await _customerService.GetCustomerById(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(StatusCode.NotFound);
            result.Data.ShouldBeNull();
            result.ErrorMessage.ShouldNotBeNull();
            result.ErrorMessage.Code.ShouldBe(expectedError.Code);
            result.ErrorMessage.Message.ShouldBe(expectedError.Message);
            result.ErrorMessage.Parameters.ShouldNotBeNull();
            result.ErrorMessage.Parameters.Keys.ShouldBe(expectedError.Parameters!.Keys);
            result.ErrorMessage.Parameters.Values.ShouldBe(expectedError.Parameters.Values);
            _customerRepository.Verify(c => c.GetById(id), Times.Once());
            _customerRepository.VerifyNoOtherCalls();
        }

        private readonly CustomerService _customerService;
        private readonly Mock<ICustomerRepository> _customerRepository;

        public CustomerServiceTests()
        {
            _customerRepository = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_customerRepository.Object);
        }
    }
}
