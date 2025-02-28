using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using OrderManager.UI.Components;
using OrderManager.UI.Services;
using OrderManager.UI.UnitTests.Common;
using Shouldly;

namespace OrderManager.UI.UnitTests.Components
{
    public class CartIndicatorComponentTests
    {
        [Fact]
        public void ShouldRenderComponent()
        {
            // Arrange Act
            var cartIndicatorComponent = _testContext.RenderComponent<CartIndicatorComponent>();

            // Assert
            cartIndicatorComponent.ShouldNotBeNull();
            cartIndicatorComponent.Instance.ShouldNotBeNull();
            cartIndicatorComponent.Markup.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CartEmpty_ShouldNotDisplayQuantity()
        {
            // Arrange Act
            var cartIndicatorComponent = _testContext.RenderComponent<CartIndicatorComponent>();

            // Assert
            cartIndicatorComponent.ShouldNotBeNull();
            var cartItemsQuantity = cartIndicatorComponent.Find("[data-name=\"cart-indicator-count\"]");
            cartItemsQuantity.ShouldNotBeNull();
            cartItemsQuantity.TextContent.ShouldBe(string.Empty);
        }

        [Fact]
        public void CartHasElements_ShouldShowQuantity()
        {
            // Arrange
            var expectedQuantity = 10;
            _cartService.Setup(c => c.GetTotalItems()).ReturnsAsync(expectedQuantity);

            // Act
            var cartIndicatorComponent = _testContext.RenderComponent<CartIndicatorComponent>();

            // Assert
            cartIndicatorComponent.ShouldNotBeNull();
            var cartItemsQuantity = cartIndicatorComponent.Find("[data-name=\"cart-indicator-count\"]");
            cartItemsQuantity.ShouldNotBeNull();
            cartItemsQuantity.TextContent.ShouldBe(expectedQuantity.ToString());
        }

        [Fact]
        public async Task CartElementsChanged_ShouldShowQuantity()
        {
            // Arrange
            var initQuantity = 0;
            _cartService.Setup(c => c.GetTotalItems()).ReturnsAsync(initQuantity);
            var cartIndicatorComponent = _testContext.RenderComponent<CartIndicatorComponent>();
            var cartItemsQuantity = cartIndicatorComponent.Find("[data-name=\"cart-indicator-count\"]");
            int.TryParse(cartItemsQuantity.TextContent, out var previousQuantity).ShouldBeFalse();
            var expectedQuantity = 10;

            // Act
            await cartIndicatorComponent.InvokeAsync(() => _cartService.Raise(m => m.OnCartItemsChanged += null, expectedQuantity));

            // Assert
            cartIndicatorComponent.ShouldNotBeNull();
            cartItemsQuantity = cartIndicatorComponent.Find("[data-name=\"cart-indicator-count\"]");
            int.TryParse(cartItemsQuantity.TextContent, out var quantity).ShouldBeTrue();
            quantity.ShouldBeGreaterThan(previousQuantity);
            quantity.ShouldBe(expectedQuantity);
        }

        private readonly TestContext _testContext;
        private readonly Mock<ICartService> _cartService;

        public CartIndicatorComponentTests()
        {
            _cartService = new Mock<ICartService>();
            _testContext = new ConfiguredTestContext();
            _testContext.Services.AddSingleton((_) => _cartService.Object);
            var popoverProvider = _testContext.RenderComponent<MudPopoverProvider>();
            popoverProvider.ShouldNotBeNull();
            popoverProvider.Markup.ShouldNotBeNull();
        }
    }
}
