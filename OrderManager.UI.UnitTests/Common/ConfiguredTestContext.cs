using Bunit;
using MudBlazor.Services;

namespace OrderManager.UI.UnitTests.Common
{
    public class ConfiguredTestContext : TestContext
    {
        public ConfiguredTestContext()
        {
            Services.AddMudServices();
            JSInterop.Mode = JSRuntimeMode.Loose;
        }
    }
}
