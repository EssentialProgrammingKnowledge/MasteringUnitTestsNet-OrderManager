using Bunit;
using Microsoft.AspNetCore.Components;
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
using System.Globalization;

namespace OrderManager.UI.UnitTests.Components
{
    public class ProductsComponentTests
    {
        [Fact]
        public void ShouldRenderComponent()
        {
            // Arrange Act
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();

            // Assert
            productsComponent.ShouldNotBeNull();
            productsComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DataIsLoading_ShouldShowLoadingIndicator()
        {
            // Arrange
            var tcs = new TaskCompletionSource<Result<List<ProductDTO>>>();
            _productService.Setup(x => x.GetAll()).Returns(tcs.Task);

            // Act
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();

            // Assert
            var loadingIndicatorComponent = productsComponent.FindComponent<MudProgressLinear>();
            loadingIndicatorComponent.ShouldNotBeNull();
            loadingIndicatorComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DataLoaded_ShouldRenderTable()
        {
            // Arrange Act
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();

            // Assert
            var productsTable = productsComponent.FindComponent<MudTable<ProductDTO>>();
            productsTable.ShouldNotBeNull();
            productsTable.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void DataLoaded_WithCollection_ShouldRenderTableWithRecords()
        {
            // Arrange
            var products = CreateProducts();
            _productService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ProductDTO>>.Success(products));

            // Act
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();

            // Assert
            var productsTable = productsComponent.FindComponent<MudTable<ProductDTO>>();
            products.ForEach(p => productsTable.Find($"[data-name=\"products-column-id-{p.Id}\"]").ShouldNotBeNull());
        }

        [Fact]
        public void AddButtonClicked_ShouldOpenModalWithProductForm()
        {
            // Arrange
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();
            var addButton = productsComponent.Find("[data-name=\"products-add-button\"]");
            var productFormComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            addButton.Click();

            // Assert
            productFormComponent.ShouldNotBeNull();
            productFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void AddNewProduct_EmptyForm_ShouldShowErrorMessages()
        {
            // Arrange
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();
            var addButton = productsComponent.Find("[data-name=\"products-add-button\"]");
            var productFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            addButton.Click();
            var submitButton = productFormComponent.Find("[data-name=\"product-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            var editForm = productFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public void AddNewProduct_FillForm_ShouldAddAndFetchAllProducts()
        {
            // Arrange
            _productService.Setup(p => p.Add(It.IsAny<ProductDTO>()))
                .ReturnsAsync(Result.Success());
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();
            var addButton = productsComponent.Find("[data-name=\"products-add-button\"]");
            var productFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            addButton.Click();
            var productName = "NewProduct";
            var productPrice = 100;
            FillForm(productFormComponent, productName, productPrice);
            var submitButton = productFormComponent.Find("[data-name=\"product-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            _productService.Verify(p => p.Add(It.Is<ProductDTO>(p => p.ProductName == productName && p.Price == productPrice)), Times.Once());
            _productService.Verify(p => p.GetAll(), Times.Exactly(2));
            productFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public void EditButtonClicked_ShouldOpenModalWithProductForm()
        {
            // Arrange
            var products = CreateProducts();
            _productService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ProductDTO>>.Success(products));
            var firstProduct = products.First();
            _productService.Setup(p => p.GetById(firstProduct.Id)).ReturnsAsync(Result<ProductDTO?>.Success(firstProduct));
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();
            var editButton = productsComponent.Find($"[data-name=\"products-column-edit-with-id-{firstProduct.Id}\"]");
            var productFormComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            editButton.Click();

            // Assert
            productFormComponent.ShouldNotBeNull();
            productFormComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            _productService.Verify(p => p.GetById(firstProduct.Id), Times.Once());
            var productNameInput = productFormComponent.Find("input[data-name=\"product-form-product-name\"]");
            var productPriceInput = productFormComponent.Find("input[data-name=\"product-form-price\"]");
            productNameInput.GetAttribute("value").ShouldBe(firstProduct.ProductName);
            productPriceInput.GetAttribute("value").ShouldBe(firstProduct.Price.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void EditProduct_InvalidValues_ShouldShowErrorMessages()
        {
            // Arrange
            var products = CreateProducts();
            _productService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ProductDTO>>.Success(products));
            var firstProduct = products.First();
            _productService.Setup(p => p.GetById(firstProduct.Id)).ReturnsAsync(Result<ProductDTO?>.Success(firstProduct));
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();
            var editButton = productsComponent.Find($"[data-name=\"products-column-edit-with-id-{firstProduct.Id}\"]");
            var productFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            editButton.Click();

            // Act
            FillForm(productFormComponent, string.Empty, -1);

            // Assert
            _productService.Verify(p => p.GetById(firstProduct.Id), Times.Once());
            var editForm = productFormComponent.FindComponent<EditForm>();
            editForm.ShouldNotBeNull();
            editForm.Instance.ShouldNotBeNull();
            editForm.Instance.EditContext.ShouldNotBeNull();
            var validationMessages = editForm.Instance.EditContext.GetValidationMessages();
            validationMessages.ShouldNotBeNull().ShouldNotBeEmpty();
        }

        [Fact]
        public void EditProduct_ValidData_ShouldUpdateAndFetchAllProducts()
        {
            // Arrange
            var products = CreateProducts();
            _productService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ProductDTO>>.Success(products));
            var firstProduct = products.First();
            _productService.Setup(p => p.GetById(firstProduct.Id)).ReturnsAsync(Result<ProductDTO?>.Success(firstProduct));
            _productService.Setup(p => p.Update(It.IsAny<ProductDTO>())).ReturnsAsync(Result.Success());
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();
            var editButton = productsComponent.Find($"[data-name=\"products-column-edit-with-id-{firstProduct.Id}\"]");
            var productFormComponent = _testContext.RenderComponent<MudDialogProvider>();
            editButton.Click();
            var productName = "NewProduct";
            var productPrice = 150.25M;
            FillForm(productFormComponent, productName, productPrice);
            var submitButton = productFormComponent.Find("[data-name=\"product-form-submit\"]");

            // Act
            submitButton.Click();

            // Assert
            _productService.Verify(p => p.GetById(firstProduct.Id), Times.Once());
            _productService.Verify(p => p.Update(It.Is<ProductDTO>(p => p.ProductName == productName && p.Price == productPrice)), Times.Once());
            _productService.Verify(p => p.GetAll(), Times.Exactly(2));
            productFormComponent.Markup.ShouldBeEmpty();
        }

        [Fact]
        public void DeleteButtonClicked_ShouldOpenModal()
        {
            // Arrange
            var products = CreateProducts();
            _productService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ProductDTO>>.Success(products));
            var firstProduct = products.First();
            _productService.Setup(p => p.GetById(firstProduct.Id)).ReturnsAsync(Result<ProductDTO?>.Success(firstProduct));
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();
            var deleteButton = productsComponent.Find($"[data-name=\"products-column-delete-with-id-{firstProduct.Id}\"]");
            var deleteProductComponent = _testContext.RenderComponent<MudDialogProvider>();

            // Act
            deleteButton.Click();

            // Assert
            deleteProductComponent.ShouldNotBeNull();
            deleteProductComponent.Markup.ShouldNotBeNullOrWhiteSpace();
            var yesButton = deleteProductComponent.Find(".mud-message-box__yes-button");
            yesButton.ShouldNotBeNull();
            var noButton = deleteProductComponent.Find(".mud-message-box__no-button");
            noButton.ShouldNotBeNull();
        }

        [Fact]
        public void DeleteProduct_ShouldDeleteAndFetchAllProducts()
        {
            // Arrange
            var products = CreateProducts();
            _productService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ProductDTO>>.Success(products));
            var firstProduct = products.First();
            _productService.Setup(p => p.Delete(firstProduct.Id)).ReturnsAsync(Result.Success());
            var productsComponent = _testContext.RenderComponent<ProductsComponent>();
            var deleteButton = productsComponent.Find($"[data-name=\"products-column-delete-with-id-{firstProduct.Id}\"]");
            var deleteProductComponent = _testContext.RenderComponent<MudDialogProvider>();
            deleteButton.Click();
            var confirmDeleteButton = deleteProductComponent.Find(".mud-message-box__yes-button");

            // Act
            confirmDeleteButton.Click();

            // Assert
            deleteProductComponent.Markup.ShouldBeEmpty();
            _productService.Verify(p => p.Delete(firstProduct.Id), Times.Once());
            _productService.Verify(p => p.GetAll(), Times.Exactly(2));
        }

        private void FillForm(IRenderedComponent<MudDialogProvider> productFormComponent, string productName, decimal price)
        {
            productFormComponent.Find("input[data-name=\"product-form-product-name\"]").Input(new ChangeEventArgs() { Value = productName });
            productFormComponent.Find("input[data-name=\"product-form-price\"]").Input(new ChangeEventArgs() { Value = price.ToString(new CultureInfo("en-US")) });
        }

        private List<ProductDTO> CreateProducts()
        {
            return [
                new ProductDTO { Id = 1, ProductName = "Product#1", IsDigital = true, Price = 100M },
                new ProductDTO { Id = 2, ProductName = "Product#2", IsDigital = true, Price = 200M },
                new ProductDTO { Id = 3, ProductName = "Product#3", IsDigital = false, Price = 300M, ProductStock = new ProductStockDTO(10) }
            ];
        }

        private readonly TestContext _testContext;
        private readonly Mock<IProductService> _productService;

        public ProductsComponentTests()
        {
            _productService = new Mock<IProductService>();
            _testContext = new ConfiguredTestContext();
            _testContext.Services.AddSingleton<ITranslateService, TranslateService>();
            _testContext.Services.AddScoped((_) => _productService.Object);
            _productService.Setup(p => p.GetAll()).ReturnsAsync(Result<List<ProductDTO>>.Success([]));
            var popoverProvider = _testContext.RenderComponent<MudPopoverProvider>();
            popoverProvider.ShouldNotBeNull();
            popoverProvider.Markup.ShouldNotBeNull();
        }
    }
}
