using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using OrderManager.UI.Components;
using OrderManager.UI.Models;
using OrderManager.UI.UnitTests.Common;
using Shouldly;

namespace OrderManager.UI.UnitTests.Components
{
    public class CustomerFormComponentTests
    {
        [Fact]
        public async Task RenderCustomerForm_WithRequiredFields_ShouldRenderInputFields()
        {
            // Arrange Act
            var result = await CreateComponent();

            // Assert
            var firstNameInput = result.CustomerFormComponent.Find("input[data-name='customer-form-firstname']");
            var lastNameInput = result.CustomerFormComponent.Find("input[data-name='customer-form-lastname']");
            var emailInput = result.CustomerFormComponent.Find("input[data-name='customer-form-email']");
            firstNameInput.ShouldNotBeNull();
            lastNameInput.ShouldNotBeNull();
            emailInput.ShouldNotBeNull();
        }

        [Fact]
        public async Task PopulateCustomerForm_CustomerIsSet_ShouldFillFormWithCustomerData()
        {
            // Arrange
            var customer = new CustomerDTO
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var parameters = new DialogParameters { ["Customer"] = customer };

            // Act
            var result = await CreateComponent(dialogParameters: parameters);

            // Assert
            var firstNameInput = result.CustomerFormComponent.Find("input[data-name='customer-form-firstname']").GetAttribute("value");
            var lastNameInput = result.CustomerFormComponent.Find("input[data-name='customer-form-lastname']").GetAttribute("value");
            var emailInput = result.CustomerFormComponent.Find("input[data-name='customer-form-email']").GetAttribute("value");
            firstNameInput.ShouldBe("John");
            lastNameInput.ShouldBe("Doe");
            emailInput.ShouldBe("john.doe@example.com");
        }

        [Fact]
        public async Task ClickSaveButton_FormIsValid_ShouldSubmitDataAndCloseDialog()
        {
            // Arrange
            var customer = new CustomerDTO
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var parameters = new DialogParameters { ["Customer"] = customer };
            var result = await CreateComponent(dialogParameters: parameters);
            var submitButton = result.CustomerFormComponent.Find("button[data-name='customer-form-submit']");

            // Act
            submitButton.Click();

            // Assert
            var dialogResult = await result.DialogReference.Result;
            dialogResult.ShouldNotBeNull();
            dialogResult.Canceled.ShouldBeFalse();
            var newCustomer = dialogResult.Data as CustomerDTO;
            newCustomer.ShouldNotBeNull();
            newCustomer.FirstName.ShouldBe(customer.FirstName);
            newCustomer.LastName.ShouldBe(customer.LastName);
            newCustomer.Email.ShouldBe(customer.Email);
            result.CustomerFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public async Task ClickSaveButton_ChangedDataAndFormIsValid_ShouldSubmitDataAndCloseDialog()
        {
            // Arrange
            var customer = new CustomerDTO
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var parameters = new DialogParameters { ["Customer"] = customer };
            var result = await CreateComponent(dialogParameters: parameters);
            var submitButton = result.CustomerFormComponent.Find("button[data-name='customer-form-submit']");
            var newFirstName = "Staszek";
            var newLastName = "Kors";
            var newEmail = "staszek.kors@kurs.pl";
            FillForm(result.CustomerFormComponent, newFirstName, newLastName, newEmail);

            // Act
            submitButton.Click();

            // Assert
            var dialogResult = await result.DialogReference.Result;
            dialogResult.ShouldNotBeNull();
            dialogResult.Canceled.ShouldBeFalse();
            var newCustomer = dialogResult.Data as CustomerDTO;
            newCustomer.ShouldNotBeNull();
            newCustomer.FirstName.ShouldBe(newFirstName);
            newCustomer.LastName.ShouldBe(newLastName);
            newCustomer.Email.ShouldBe(newEmail);
            result.CustomerFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public async Task ClickCancelButton_ShouldCloseDialogWithoutSaving()
        {
            // Arrange
            var customer = new CustomerDTO
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var parameters = new DialogParameters { ["Customer"] = customer };
            var result = await CreateComponent(dialogParameters: parameters);
            var cancelButton = result.CustomerFormComponent.Find("button[data-name='customer-form-cancel']");

            // Act
            cancelButton.Click();

            // Assert
            var dialogResult = await result.DialogReference.Result;
            dialogResult.ShouldNotBeNull();
            dialogResult.Canceled.ShouldBeTrue();
        }

        [Fact]
        public async Task Submit_EmptyForm_ShouldShowValidationErrorMessages()
        {
            // Arrange
            var result = await CreateComponent();
            var submitButton = result.CustomerFormComponent.Find("button[data-name='customer-form-submit']");

            // Act
            submitButton.Click();

            // Assert
            var editForm = result.CustomerFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public async Task ChangeInputValues_ShouldUpdateFormFields()
        {
            // Arrange
            var customer = new CustomerDTO
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var parameters = new DialogParameters { ["Customer"] = customer };
            var result = await CreateComponent(dialogParameters: parameters);
            var firstNameInput = result.CustomerFormComponent.Find("input[data-name='customer-form-firstname']");

            // Act
            firstNameInput.Change("Jane");

            // Assert
            var updatedFirstName = result.CustomerFormComponent.Find("input[data-name='customer-form-firstname']").GetAttribute("value");
            updatedFirstName.ShouldBe("Jane");
        }

        [Fact]
        public async Task RenderElements_CorrectDataNameAttributes_ShouldRenderRequiredAttributes()
        {
            // Arrange
            var customer = new CustomerDTO
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var parameters = new DialogParameters { ["Customer"] = customer };

            // Act
            var result = await CreateComponent(dialogParameters: parameters);

            // Act
            result.CustomerFormComponent.Find("input[data-name='customer-form-firstname']").ShouldNotBeNull();
            result.CustomerFormComponent.Find("input[data-name='customer-form-lastname']").ShouldNotBeNull();
            result.CustomerFormComponent.Find("input[data-name='customer-form-email']").ShouldNotBeNull();
            result.CustomerFormComponent.Find("button[data-name='customer-form-submit']").ShouldNotBeNull();
            result.CustomerFormComponent.Find("button[data-name='customer-form-cancel']").ShouldNotBeNull();
        }

        private void FillForm(IRenderedComponent<MudDialogProvider> customerForm, string firstName, string lastName, string email)
        {
            customerForm.Find("input[data-name=\"customer-form-firstname\"]").Change(firstName);
            customerForm.Find("input[data-name=\"customer-form-lastname\"]").Change(lastName);
            customerForm.Find("input[data-name=\"customer-form-email\"]").Change(email);
        }

        private async Task<(IRenderedComponent<MudDialogProvider> CustomerFormComponent, IDialogReference DialogReference)> CreateComponent(string? title = null, DialogParameters? dialogParameters = null)
        {
            var dialogService = _testContext.Services.GetRequiredService<IDialogService>();
            var productFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            var dialogReference = dialogParameters is not null ?
                await dialogService.ShowAsync<CustomerFormComponent>(title, dialogParameters)
                : await dialogService.ShowAsync<CustomerFormComponent>(title);
            return (productFormComponent, dialogReference);
        }

        private readonly TestContext _testContext;

        public CustomerFormComponentTests()
        {
            _testContext = new ConfiguredTestContext();
        }
    }
}
