using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace OrderManager.UI.UnitTests.Common
{
    public class DummyComponent : ComponentBase
    {
        [Parameter] public string Text { get; set; } = "Hello";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "p");
            builder.AddContent(1, Text);
            builder.CloseElement();
        }
    }
}
