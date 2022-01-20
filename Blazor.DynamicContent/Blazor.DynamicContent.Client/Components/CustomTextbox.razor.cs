using Microsoft.AspNetCore.Components;

namespace Blazor.DynamicContent.Client.Components
{
    public partial class CustomTextbox
    {
        [Parameter] public string Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }
        [Parameter] public string Label { get; set; }
        [Parameter] public string EmptyText { get; set; }
        [Parameter] public string ErrorText { get; set; }
        [Parameter] public bool Required { get; set; }
    }
}