using Microsoft.AspNetCore.Components;

namespace Blazor.DynamicContent.Client.Components
{
    public partial class CustomCheckbox
    {
        [Parameter] public bool Checked { get; set; }
        [Parameter] public bool Required { get; set; }
        [Parameter] public string Label { get; set; }
    }
}