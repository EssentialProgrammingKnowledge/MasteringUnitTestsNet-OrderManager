using Bunit;
using Microsoft.AspNetCore.Components;
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
    public class OrderEditComponentTests
    {
        [Fact]
        public void ShouldRenderComponent()
        {
            // Arrange Act
            var orderEditComponent = _testContext.RenderComponent<OrderEditComponent>();

            // Assert
            orderEditComponent.ShouldNotBeNull();
            orderEditComponent.Instance.ShouldNotBeNull();
            orderEditComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void LoadingData_ShouldDisplayLoadingIndicator()
        {
            // Arrange
            var tcs = new TaskCompletionSource<Result<OrderDetailsDTO?>>();
            _mockOrderService.Setup(o => o.GetById(It.IsAny<int>())).Returns(tcs.Task);

            // Act
            var component = _testContext.RenderComponent<OrderEditComponent>();

            // Assert
            var loadingIcon = component.Find("[data-name='order-edit-loading-icon']");
            loadingIcon.ShouldNotBeNull();
            var loadingIconComponent = component.FindComponent<MudProgressLinear>();
            loadingIconComponent.ShouldNotBeNull();
        }

        [Fact]
        public void LoadOrder_OrderIsNull_ShouldDisplayNoOrderFound()
        {
            // Arrange
            _mockOrderService.Setup(service => service.GetById(It.IsAny<int>())).ReturnsAsync(Result<OrderDetailsDTO?>.Success(null));

            // Act
            var component = _testContext.RenderComponent<OrderEditComponent>(
                parameters => parameters.Add(p => p.Id, 1)
            );

            // Assert
            var notFoundInfo = component.Find("[data-name='order-edit-not-found-info']");
            var goToOrdersButton = component.Find("[data-name='order-edit-go-to-orders-button']");
            notFoundInfo.ShouldNotBeNull();
            goToOrdersButton.ShouldNotBeNull();
        }

        [Fact]
        public void OrderLoaded_CanBeEdited_ShouldEnableSaveButton()
        {
            // Arrange
            var order = new OrderDetailsDTO(1, "OrderNumber#1", 100M, OrderStatus.New, DateTime.UtcNow, new CustomerDTO { Id = 1, FirstName = "John", LastName = "Doe" }, []);
            _mockOrderService.Setup(service => service.GetById(It.IsAny<int>())).ReturnsAsync(Result<OrderDetailsDTO?>.Success(order));

            // Act
            var component = _testContext.RenderComponent<OrderEditComponent>(
                parameters => parameters.Add(p => p.Id, 1)
            );

            // Assert
            var saveButton = component.Find("[data-name='order-edit-submit-button']");
            saveButton.ShouldNotBeNull();
            saveButton.GetAttribute("disabled").ShouldBeNull();
        }

        [Fact]
        public async Task CustomerChange_UsingMudSelect_ShouldUpdateCustomerInOrderToUpdateDto()
        {
            // Arrange
            var customers = new List<CustomerDTO>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };
            var order = new OrderDetailsDTO(1, "OrderNumber#1", 100M, OrderStatus.New, DateTime.UtcNow, new CustomerDTO { Id = 1, FirstName = "John", LastName = "Doe" }, []);
            _mockOrderService.Setup(service => service.GetById(It.IsAny<int>())).ReturnsAsync(Result<OrderDetailsDTO?>.Success(order));
            _mockCustomerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var component = _testContext.RenderComponent<OrderEditComponent>(
                parameters => parameters.Add(p => p.Id, 1)
            );
            var customerSelect = component.FindComponents<MudSelect<int>>()
                .FirstOrDefault(c => c.Find("[data-name='order-edit-customer-data-selected-cutomer']") is not null);
            customerSelect.ShouldNotBeNull();
            var expectedCustomerId = 2;

            // Act
            await component.InvokeAsync(async () => await customerSelect.Instance.ValueChanged.InvokeAsync(expectedCustomerId));

            // Assert
            var customerUpdated = component.Find("[data-name='order-edit-customer-data-selected-cutomer']");
            customerUpdated.ShouldNotBeNull();
            customerUpdated.GetAttribute("value").ShouldBe(expectedCustomerId.ToString());
        }

        [Fact]
        public void CancelButtonClicked_ShouldNavigateToOrdersPage()
        {
            // Arrange
            var customers = new List<CustomerDTO>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };
            var order = new OrderDetailsDTO(1, "OrderNumber#1", 100M, OrderStatus.New, DateTime.UtcNow, new CustomerDTO { Id = 1, FirstName = "John", LastName = "Doe" }, []);
            _mockOrderService.Setup(service => service.GetById(It.IsAny<int>())).ReturnsAsync(Result<OrderDetailsDTO?>.Success(order));
            _mockCustomerService.Setup(c => c.GetAll()).ReturnsAsync(Result<List<CustomerDTO>>.Success(customers));
            var component = _testContext.RenderComponent<OrderEditComponent>(
                parameters => parameters.Add(p => p.Id, 1)
            ); 
            var cancelButton = component.Find("[data-name='order-edit-cancel-button']");

            // Act
            cancelButton.Click();

            // Assert
            _navigationManager.LastNavigatedUrl.ShouldBe("/orders");
        }

        private readonly TestContext _testContext;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IDialogService> _mockDialogService;
        private readonly Mock<ISnackbar> _mockSnackbar;
        private readonly FakeNavigationManager _navigationManager;
        private readonly Mock<ICustomerService> _mockCustomerService;

        public OrderEditComponentTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockDialogService = new Mock<IDialogService>();
            _mockSnackbar = new Mock<ISnackbar>();
            _navigationManager = new FakeNavigationManager();
            _mockCustomerService = new Mock<ICustomerService>();
            _testContext = new ConfiguredTestContext();
            _testContext.Services.AddTranslations();
            _testContext.Services.AddScoped((_) => _mockOrderService.Object);
            _testContext.Services.AddScoped((_) => _mockDialogService.Object);
            _testContext.Services.AddScoped((_) => _mockSnackbar.Object);
            _testContext.Services.AddSingleton<NavigationManager>((_) => _navigationManager);
            _testContext.Services.AddScoped((_) => _mockCustomerService.Object);
            var popoverProvider = _testContext.RenderComponent<MudPopoverProvider>();
            popoverProvider.ShouldNotBeNull();
            popoverProvider.Markup.ShouldNotBeNull();
        }
    }
}
