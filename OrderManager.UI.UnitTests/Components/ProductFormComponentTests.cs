using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using OrderManager.UI.Components;
using OrderManager.UI.Models;
using OrderManager.UI.UnitTests.Common;
using Shouldly;
using System.Globalization;

namespace OrderManager.UI.UnitTests.Components
{
    public class ProductFormComponentTests
    {
        [Fact]
        public async Task ShouldRenderComponent()
        {
            // Arrange Act
            var createdComponent = await CreateComponent();

            // Assert
            createdComponent.ProductFormComponent.ShouldNotBeNull();
            createdComponent.ProductFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            createdComponent.DialogReference.ShouldNotBeNull();
        }

        [Fact]
        public async Task SubmitForm_EmptyForm_ShouldReturnValidationError()
        {
            // Arrange
            var createdComponent = await CreateComponent();
            var submitButton = createdComponent.ProductFormComponent.Find("button[data-name=\"product-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            createdComponent.ProductFormComponent.ShouldNotBeNull();
            createdComponent.ProductFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            var mudInputErrors = createdComponent.ProductFormComponent.FindAll("div.mud-input-helper-text.mud-input-error");
            mudInputErrors.ShouldNotBeNull();
            mudInputErrors.ShouldNotBeEmpty();
            mudInputErrors.Count.ShouldBe(2);
        }

        [Fact]
        public async Task SubmitForm_ValidData_ShouldCloseDialogWithCorrectData()
        {
            // Arrange
            var createdComponent = await CreateComponent();
            createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-product-name\"]").Input(new ChangeEventArgs() { Value = "Test Product" });
            createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-price\"]").Input(new ChangeEventArgs() { Value = "99.99" });
            createdComponent.ProductFormComponent.Find("button[data-name=\"product-form-submit\"]").Click();

            // Act
            var result = (await createdComponent.DialogReference!.Result)?.Data as ProductDTO;

            // Assert
            result.ShouldNotBeNull();
            result.ProductName.ShouldBe("Test Product");
            result.Price.ShouldBe(99.99M);
        }

        [Fact]
        public async Task ChangeProductType_SwitchToDigital_ShouldEnableQuantity()
        {
            // Arrange
            var createdComponent = await CreateComponent();
            var digitalSwitch = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-is-digital\"]");
            var quantityField = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-quantity\"]");

            // Act
            digitalSwitch.Change(true);
            
            // Assert
            quantityField.HasAttribute("disabled").ShouldBeTrue();
        }

        [Fact]
        public async Task ChangeProductType_SwitchToStock_ShouldDisableQuantity()
        {
            // Arrange
            var createdComponent = await CreateComponent();
            var digitalSwitch = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-is-digital\"]");
            var quantityField = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-quantity\"]");

            // Act
            digitalSwitch.Change(false);

            // Assert
            quantityField.HasAttribute("disabled").ShouldBeFalse();
        }

        [Fact]
        public async Task Cancel_ShouldCloseDialogWithoutSaving()
        {
            // Arrange
            var createdComponent = await CreateComponent();

            // Act
            createdComponent.ProductFormComponent.Find("button[data-name=\"product-form-cancel\"]").Click();

            // Assert
            var result = await createdComponent.DialogReference.Result;
            result.ShouldNotBeNull();
            result.Canceled.ShouldBeTrue();
        }

        [Fact]
        public async Task FillForm_ProductIsProvided_ShouldPopulateFields()
        {
            // Arrange
            var existingProduct = new ProductDTO
            {
                Id = 1,
                ProductName = "Existing Product",
                Price = 199.99M,
                IsDigital = false,
                ProductStock = new ProductStockDTO(10)
            };
            var parameters = new DialogParameters { ["Product"] = existingProduct };

            // Act
            var createdComponent = await CreateComponent("Edytuj produkt", parameters);

            // Assert
            var nameField = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-product-name\"]");
            var priceField = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-price\"]");
            var quantityField = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-quantity\"]");
            var digitalSwitch = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-is-digital\"]");
            nameField.GetAttribute("value").ShouldBe(existingProduct.ProductName);
            decimal.TryParse(priceField.GetAttribute("value"), CultureInfo.InvariantCulture, out var priceChanged).ShouldBeTrue();
            priceChanged.ShouldBe(existingProduct.Price);
            decimal.TryParse(quantityField.GetAttribute("value"), CultureInfo.InvariantCulture, out var quantityUpdated).ShouldBeTrue();
            quantityUpdated.ShouldBe(existingProduct.ProductStock.Quantity);
            quantityField.HasAttribute("disabled").ShouldBeFalse();
            digitalSwitch.HasAttribute("checked").ShouldBeFalse();
        }

        [Fact]
        public async Task FillForm_DigitalProductIsProvided_ShouldPopulateFields()
        {
            // Arrange
            var existingProduct = new ProductDTO
            {
                Id = 1,
                ProductName = "Existing Product",
                Price = 199.99M,
                IsDigital = true
            };
            var parameters = new DialogParameters { ["Product"] = existingProduct };

            // Act
            var createdComponent = await CreateComponent("Edytuj produkt", parameters);

            // Assert
            var nameField = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-product-name\"]");
            var priceField = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-price\"]");
            var quantityField = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-quantity\"]");
            var digitalSwitch = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-is-digital\"]");
            nameField.GetAttribute("value").ShouldBe(existingProduct.ProductName);
            decimal.TryParse(priceField.GetAttribute("value"), CultureInfo.InvariantCulture, out var priceChanged).ShouldBeTrue();
            priceChanged.ShouldBe(existingProduct.Price);
            decimal.TryParse(quantityField.GetAttribute("value"), CultureInfo.InvariantCulture, out var quantityUpdated).ShouldBeTrue();
            quantityUpdated.ShouldBe(decimal.Zero);
            quantityField.HasAttribute("disabled").ShouldBeTrue();
            digitalSwitch.HasAttribute("checked").ShouldBeTrue();
        }

        [Fact]
        public async Task EditForm_UserModifiesData_ShouldUpdateValues()
        {
            // Arrange
            var existingProduct = new ProductDTO
            {
                Id = 2,
                ProductName = "Old Name",
                Price = 49.99M,
                IsDigital = false,
                ProductStock = new ProductStockDTO(5)
            };
            var parameters = new DialogParameters { ["Product"] = existingProduct };
            var createdComponent = await CreateComponent("Edytuj produkt", parameters);
            var newProductName = "New Name";
            var newPice = 59.99M;

            // Act
            FillFormAndSubmit(createdComponent.ProductFormComponent, newProductName, newPice);

            // Assert
            var result = await createdComponent.DialogReference.Result;
            result.ShouldNotBeNull();
            var data = result.Data as ProductDTO;
            data.ShouldNotBeNull();
            data.ProductName.ShouldBe(newProductName);
            data.Price.ShouldBe(newPice);
        }

        [Fact]
        public async Task EditForm_EditingDigitalProduct_ShouldDisableQuantity()
        {
            // Arrange
            var digitalProduct = new ProductDTO
            {
                Id = 3,
                ProductName = "Digital Product",
                Price = 29.99M,
                IsDigital = true,
                ProductStock = null
            };
            var parameters = new DialogParameters { ["Product"] = digitalProduct };
            var createdComponent = await CreateComponent("Edytuj produkt", parameters);

            // Act
            var quantityField = createdComponent.ProductFormComponent.Find("input[data-name=\"product-form-quantity\"]");

            // Assert
            quantityField.HasAttribute("disabled").ShouldBeTrue();
        }

        [Fact]
        public async Task SubmitForm_EditingProductUserModifiesData_ShouldUpdateValuesAndCloseDialogWithCorrectData()
        {
            // Arrange
            var existingProduct = new ProductDTO
            {
                Id = 2,
                ProductName = "Old Name",
                Price = 49.99M,
                IsDigital = false,
                ProductStock = new ProductStockDTO(5)
            };
            var parameters = new DialogParameters { ["Product"] = existingProduct };
            var createdComponent = await CreateComponent("Edytuj produkt", parameters);
            var newProductName = "New Name";
            var newPice = 59.99M;

            // Act
            FillFormAndSubmit(createdComponent.ProductFormComponent, newProductName, newPice);

            // Assert
            var result = await createdComponent.DialogReference.Result;
            result.ShouldNotBeNull();
            var data = result.Data as ProductDTO;
            data.ShouldNotBeNull();
            data.ProductName.ShouldBe(newProductName);
            data.Price.ShouldBe(newPice);
        }

        private void FillFormAndSubmit(IRenderedComponent<MudDialogProvider> productFormComponent, string productName, decimal price)
        {
            FillForm(productFormComponent, productName, price);
            productFormComponent.Find("button[data-name=\"product-form-submit\"]").Click();
        }

        private void FillForm(IRenderedComponent<MudDialogProvider> productFormComponent, string productName, decimal price)
        {
            productFormComponent.Find("input[data-name=\"product-form-product-name\"]").Input(new ChangeEventArgs() { Value = productName });
            productFormComponent.Find("input[data-name=\"product-form-price\"]").Input(new ChangeEventArgs() { Value = price.ToString(new CultureInfo("en-US")) });
        }

        private async Task<(IRenderedComponent<MudDialogProvider> ProductFormComponent, IDialogReference DialogReference)> CreateComponent(string? title = null, DialogParameters? dialogParameters = null)
        {
            var dialogService = _testContext.Services.GetRequiredService<IDialogService>();
            var productFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            var dialogReference = dialogParameters is not null ?
                await dialogService.ShowAsync<ProductFormComponent>(title, dialogParameters)
                : await dialogService.ShowAsync<ProductFormComponent>(title);
            return (productFormComponent, dialogReference);
        }

        private readonly TestContext _testContext;

        public ProductFormComponentTests()
        {
            _testContext = new ConfiguredTestContext();
        }
    }
}
