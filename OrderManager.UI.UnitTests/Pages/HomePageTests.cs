using Bunit;
using MudBlazor.Services;
using OrderManager.UI.Components;
using OrderManager.UI.Pages;
using OrderManager.UI.UnitTests.Common;
using Shouldly;

namespace OrderManager.UI.UnitTests.Pages
{
    public class HomePageTests
    {
        [Fact]
        public void ShouldRenderPage()
        {
            // Arrange Act
            var homePage = _testContext.RenderComponent<Home>();

            // Assert
            homePage.ShouldNotBeNull();
            homePage.Markup.ShouldNotBeNullOrWhiteSpace();
            var homeInfo = homePage.Find("[data-name=\"home-page-info\"]");
            homeInfo.ShouldNotBeNull();
            homeInfo.TextContent.ShouldNotBeNullOrWhiteSpace();
            homeInfo.TextContent.ShouldContain("Lista produktów");
            homeInfo.TextContent.ShouldContain("Przeglądaj i zarządzaj dostępnymi produktami w systemie");
        }

        private readonly TestContext _testContext;

        public HomePageTests()
        {
            _testContext = new ConfiguredTestContext();
            _testContext.Services.AddMudServices();
            _testContext.ComponentFactories.Add(type => type == typeof(ProductsList),
                _ => new DummyComponent());
        }
    }
}
