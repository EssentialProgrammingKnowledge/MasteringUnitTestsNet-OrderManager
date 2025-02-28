using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using OrderManager.UI.Components;
using OrderManager.UI.Languages;
using OrderManager.UI.Models;
using OrderManager.UI.Services;
using OrderManager.UI.UnitTests.Common;
using Shouldly;

namespace OrderManager.UI.UnitTests.Components
{
    public class CustomersComponentTests
    {
        [Fact]
        public void ShouldRenderComponent()
        {
            // Arrange Act
            var customersComponent = _testContext.RenderComponent<CustomersComponent>();

            // Assert
            customersComponent.ShouldNotBeNull();
            customersComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DataIsLoading_ShouldShowLoadingIndicator()
        {
            // Arrange
            var tcs = new TaskCompletionSource<Result<List<CustomerDTO>>>();
            _customerService.Setup(x => x.GetAll()).Returns(tcs.Task);

            // Act
            var customersComponent = _testContext.RenderComponent<CustomersComponent>();

            // Assert
            var loadingIndicatorComponent = customersComponent.FindComponent<MudProgressCircular>();
            loadingIndicatorComponent.ShouldNotBeNull();
            loadingIndicatorComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void LoadCustomers_ShouldPopulateCustomerList()
        {
            // Arrange
            var customers = new List<CustomerDTO>
            {
                new () { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                new () { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" }
            };
            _customerService.Setup(s => s.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));

            // Act
            var component = _testContext.RenderComponent<CustomersComponent>();

            // Assert
            component.FindAll("tr").Count.ShouldBe(customers.Count + 1);
        }

        [Fact]
        public void SearchCustomer_ShouldFilterResults()
        {
            // Arrange
            var customers = new List<CustomerDTO>
            {
                new () { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                new () { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" }
            };
            _customerService.Setup(s => s.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var component = _testContext.RenderComponent<CustomersComponent>();
            var searchInput = component.Find("input[data-name=\"customers-search-input\"]");

            // Act
            searchInput.Change("Jane");

            // Assert
            component.FindAll("tr").Count.ShouldBe(customers.Count + 1);
        }

        [Fact]
        public void ClickAddButton_ShouldOpenCustomerFormModal()
        {
            // Arrange
            var component = _testContext.RenderComponent<CustomersComponent>();
            var addButton = component.Find("[data-name=\"customers-add-button\"]");
            var customerFormComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            addButton.Click();

            // Assert
            customerFormComponent.ShouldNotBeNull();
            customerFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void AddNewCustomer_InvalidData_ShouldCallAddCustomerService()
        {
            // Arrange
            var component = _testContext.RenderComponent<CustomersComponent>();
            var addButton = component.Find("[data-name=\"customers-add-button\"]");
            var customerFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            addButton.Click();
            var submitButton = customerFormComponent.Find("[data-name=\"customer-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            var editForm = customerFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public void AddNewCustomer_ErrorWhileSendData_ShouldShowErrorSnackbar()
        {
            // Arrange
            _customerService.Setup(c => c.Add(It.IsAny<CustomerDTO>())).ReturnsAsync(Result.Failed(new ErrorMessage("CODE", "MSG")));
            var component = _testContext.RenderComponent<CustomersComponent>();
            var addButton = component.Find("[data-name=\"customers-add-button\"]");
            var customerFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            var newCustomer = new CustomerDTO { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            addButton.Click();
            var submitButton = customerFormComponent.Find("[data-name=\"customer-form-submit\"]");
            FillForm(customerFormComponent, newCustomer.FirstName, newCustomer.LastName, newCustomer.Email);

            // Act
            submitButton.Click();

            // Assert
            _snackbar.Verify(s => s.Add(It.IsAny<string>(), Severity.Error, It.IsAny<Action<SnackbarOptions>>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void AddNewCustomer_ValidData_ShouldCallAddCustomerService()
        {
            // Arrange
            _customerService.Setup(c => c.Add(It.IsAny<CustomerDTO>()))
                .ReturnsAsync(Result.Success());
            var component = _testContext.RenderComponent<CustomersComponent>();
            var addButton = component.Find("[data-name=\"customers-add-button\"]");
            var customerFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            var newCustomer = new CustomerDTO { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            addButton.Click();
            var submitButton = customerFormComponent.Find("[data-name=\"customer-form-submit\"]");
            FillForm(customerFormComponent, newCustomer.FirstName, newCustomer.LastName, newCustomer.Email);

            // Act
            submitButton.Click();

            // Assert
            _customerService.Verify(c => c.Add(It.Is<CustomerDTO>(c => c.FirstName == newCustomer.FirstName && c.LastName == newCustomer.LastName && c.Email == newCustomer.Email)), Times.Once());
            _customerService.Verify(c => c.GetAll(), Times.Exactly(2));
            customerFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public void EditButtonClicked_ShouldOpenModalWithCustomerForm()
        {
            // Arrange
            var customers = CreateCustomers();
            _customerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var firstCustomer = customers.First();
            _customerService.Setup(c => c.GetById(firstCustomer.Id)).ReturnsAsync(Result<CustomerDTO?>.Success(firstCustomer));
            var customersComponent = _testContext.RenderComponent<CustomersComponent>();
            var editButton = customersComponent.Find($"[data-name=\"customers-column-edit-with-id-{firstCustomer.Id}\"]");
            var customerFormComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            editButton.Click();

            // Assert
            customerFormComponent.ShouldNotBeNull();
            customerFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            _customerService.Verify(c => c.GetById(firstCustomer.Id), Times.Once());
            var firstNameInput = customerFormComponent.Find("input[data-name=\"customer-form-firstname\"]");
            var lastNameInput = customerFormComponent.Find("input[data-name=\"customer-form-lastname\"]");
            var emailInput = customerFormComponent.Find("input[data-name=\"customer-form-email\"]");
            firstNameInput.GetAttribute("value").ShouldBe(firstCustomer.FirstName);
            lastNameInput.GetAttribute("value").ShouldBe(firstCustomer.LastName);
            emailInput.GetAttribute("value").ShouldBe(firstCustomer.Email);
        }

        [Fact]
        public void EditCustomer_InvalidValues_ShouldShowErrorMessages()
        {
            // Arrange
            var customer = CreateCustomers();
            _customerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customer));
            var firstCustomer = customer.First();
            _customerService.Setup(c => c.GetById(firstCustomer.Id)).ReturnsAsync(Result<CustomerDTO?>.Success(firstCustomer));
            var customersComponent = _testContext.RenderComponent<CustomersComponent>();
            var editButton = customersComponent.Find($"[data-name=\"customers-column-edit-with-id-{firstCustomer.Id}\"]");
            var customerFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            editButton.Click();

            // Act
            FillForm(customerFormComponent, string.Empty, string.Empty, string.Empty);

            // Assert
            _customerService.Verify(c => c.GetById(firstCustomer.Id), Times.Once());
            var editForm = customerFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public void EditCustomer_ErrorWhileSendData_ShouldShowErrorSnackbar()
        {
            // Arrange
            var customers = CreateCustomers();
            _customerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var firstCustomer = customers.First();
            _customerService.Setup(c => c.GetById(firstCustomer.Id)).ReturnsAsync(Result<CustomerDTO?>.Success(firstCustomer));
            _customerService.Setup(c => c.Update(It.IsAny<CustomerDTO>())).ReturnsAsync(Result.Failed(new ErrorMessage("CODE", "MSG")));
            var customersComponent = _testContext.RenderComponent<CustomersComponent>();
            var editButton = customersComponent.Find($"[data-name=\"customers-column-edit-with-id-{firstCustomer.Id}\"]");
            var customerFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            editButton.Click();
            var firstName = "NewFirstName";
            var lastName = "NewLastName";
            var email = "newEmail@gmail.com";
            FillForm(customerFormComponent, firstName, lastName, email);
            var submitButton = customerFormComponent.Find("[data-name=\"customer-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            _snackbar.Verify(s => s.Add(It.IsAny<string>(), Severity.Error, It.IsAny<Action<SnackbarOptions>>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void EditCustomer_ValidData_ShouldUpdateAndFetchAllCustomers()
        {
            // Arrange
            var customers = CreateCustomers();
            _customerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var firstCustomer = customers.First();
            _customerService.Setup(c => c.GetById(firstCustomer.Id)).ReturnsAsync(Result<CustomerDTO?>.Success(firstCustomer));
            _customerService.Setup(c => c.Update(It.IsAny<CustomerDTO>())).ReturnsAsync(Result.Success());
            var customersComponent = _testContext.RenderComponent<CustomersComponent>();
            var editButton = customersComponent.Find($"[data-name=\"customers-column-edit-with-id-{firstCustomer.Id}\"]");
            var customerFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            editButton.Click();
            var firstName = "NewFirstName";
            var lastName = "NewLastName";
            var email = "newEmail@gmail.com";
            FillForm(customerFormComponent, firstName, lastName, email);
            var submitButton = customerFormComponent.Find("[data-name=\"customer-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            _customerService.Verify(c => c.GetById(firstCustomer.Id), Times.Once());
            _customerService.Verify(c => c.Update(It.Is<CustomerDTO>(c => c.FirstName == firstName && c.LastName == lastName && c.Email == email)), Times.Once());
            _customerService.Verify(c => c.GetAll(), Times.Exactly(2));
            customerFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public void DeleteCustomer_ShouldOpenDeleteCustomerModal()
        {
            // Arrange
            var customers = CreateCustomers();
            _customerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var firstCustomer = customers.First();
            _customerService.Setup(c => c.GetById(firstCustomer.Id)).ReturnsAsync(Result<CustomerDTO?>.Success(firstCustomer));
            var customerComponent = _testContext.RenderComponent<CustomersComponent>();
            var deleteButton = customerComponent.Find($"[data-name=\"customers-column-delete-with-id-{firstCustomer.Id}\"]");
            var deleteCustomerComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            deleteButton.Click();

            // Assert
            deleteCustomerComponent.ShouldNotBeNull();
            deleteCustomerComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            var yesButton = deleteCustomerComponent.Find(".mud-message-box__yes-button");
            yesButton.ShouldNotBeNull();
            var noButton = deleteCustomerComponent.Find(".mud-message-box__no-button");
            noButton.ShouldNotBeNull();
        }

        [Fact]
        public void DeleteCustomer_ShouldCallDeleteCustomerService()
        {
            // Arrange
            var customers = CreateCustomers();
            _customerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var firstCustomer = customers.First();
            _customerService.Setup(c => c.GetById(firstCustomer.Id)).ReturnsAsync(Result<CustomerDTO?>.Success(firstCustomer));
            _customerService.Setup(c => c.Delete(firstCustomer.Id)).ReturnsAsync(Result.Success());
            var customerComponent = _testContext.RenderComponent<CustomersComponent>();
            var deleteButton = customerComponent.Find($"[data-name=\"customers-column-delete-with-id-{firstCustomer.Id}\"]");
            var deleteCustomerComponent = _testContext.RenderComponent<MudDialogProvider>();
            deleteButton.Click();
            var confirmDeleteButton = deleteCustomerComponent.Find(".mud-message-box__yes-button");

            // Act
            confirmDeleteButton.Click();

            // Assert
            deleteCustomerComponent.Markup.ShouldBeEmpty();
            _customerService.Verify(c => c.Delete(firstCustomer.Id), Times.Once());
            _customerService.Verify(c => c.GetAll(), Times.Exactly(2));
        }

        [Fact]
        public void DeleteCustomer_AnErrorOccurredWhileSendRequest_ShouldShowErrorSnackbar()
        {
            // Arrange
            var customers = CreateCustomers();
            _customerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var firstCustomer = customers.First();
            _customerService.Setup(c => c.GetById(firstCustomer.Id)).ReturnsAsync(Result<CustomerDTO?>.Success(firstCustomer));
            _customerService.Setup(c => c.Delete(firstCustomer.Id)).ReturnsAsync(Result.Failed(new ErrorMessage("CODE", "MSG")));
            var customerComponent = _testContext.RenderComponent<CustomersComponent>();
            var deleteButton = customerComponent.Find($"[data-name=\"customers-column-delete-with-id-{firstCustomer.Id}\"]");
            var deleteCustomerComponent = _testContext.RenderComponent<MudDialogProvider>();
            deleteButton.Click();
            var confirmDeleteButton = deleteCustomerComponent.Find(".mud-message-box__yes-button");

            // Act
            confirmDeleteButton.Click();

            // Assert
            _snackbar.Verify(s => s.Add(It.IsAny<string>(), Severity.Error, It.IsAny<Action<SnackbarOptions>>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void LoadedData_ShouldDisplayCorrectColumns()
        {
            // Arrange
            var customers = CreateCustomers();
            _customerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var component = _testContext.RenderComponent<CustomersComponent>();

            // Act
            var rows = component.FindAll("tr");

            // Assert
            rows.Count.ShouldBe(customers.Count + 1);
            var customer = customers[0];
            rows[1].TextContent.ShouldContain(customer.FirstName);
            rows[1].TextContent.ShouldContain(customer.LastName);
            rows[1].TextContent.ShouldContain(customer.Email);
        }

        private void FillForm(IRenderedComponent<MudDialogProvider> customerForm, string firstName, string lastName, string email)
        {
            customerForm.Find("input[data-name=\"customer-form-firstname\"]").Change(firstName);
            customerForm.Find("input[data-name=\"customer-form-lastname\"]").Change(lastName);
            customerForm.Find("input[data-name=\"customer-form-email\"]").Change(email);
        }

        private List<CustomerDTO> CreateCustomers()
        {
            return [
                new CustomerDTO { Id = 1, FirstName = "Jan", LastName = "Stormowski", Email = "j.stormowski@gmail.com" },
                new CustomerDTO { Id = 2, FirstName = "Bartek", LastName = "Cequrowski", Email = "b.cequrowski@gmail.com" },
                new CustomerDTO { Id = 3, FirstName = "Michał", LastName = "Krudowski", Email = "m.krudowski@gmail.com" }
            ];
        }


        private readonly TestContext _testContext;
        private readonly Mock<ICustomerService> _customerService = new();
        private readonly Mock<ISnackbar> _snackbar;

        public CustomersComponentTests()
        {
            _customerService = new Mock<ICustomerService>();
            _testContext = new ConfiguredTestContext();
            _testContext.Services.AddSingleton<ITranslateService, TranslateService>();
            _testContext.Services.AddScoped((_) => _customerService.Object);
            _customerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success([]));
            _snackbar = new Mock<ISnackbar>();
            _testContext.Services.AddTransient((_) => _snackbar.Object);
            var popoverProvider = _testContext.RenderComponent<MudPopoverProvider>();
            popoverProvider.ShouldNotBeNull();
            popoverProvider.Markup.ShouldNotBeNull();
        }
    }
}
