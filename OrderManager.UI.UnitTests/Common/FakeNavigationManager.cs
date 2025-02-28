using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace OrderManager.UI.UnitTests.Common
{
    public class FakeNavigationManager : NavigationManager
    {
        public string? LastNavigatedUrl { get; private set; }

        public FakeNavigationManager()
        {
            Initialize("https://order-manager-unit-tests/", "https://order-manager-unit-tests/");
        }

        protected override void NavigateToCore([StringSyntax("Uri")] string uri, bool forceLoad)
        {
            NavigateToCore(uri, new NavigationOptions { ForceLoad = forceLoad });
        }

        protected override void NavigateToCore([StringSyntax("Uri")] string uri, NavigationOptions options)
        {
            LastNavigatedUrl = uri;
        }
    }

}
