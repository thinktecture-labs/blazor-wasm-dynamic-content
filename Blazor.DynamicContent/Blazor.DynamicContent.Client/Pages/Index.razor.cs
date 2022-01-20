using Microsoft.AspNetCore.Components;

namespace Blazor.DynamicContent.Client.Pages
{
    public partial class Index
    {
        [Inject] public NavigationManager NavigationManager { get; set; }

        private void NavigateTo(string url)
        {
            NavigationManager.NavigateTo(url);
        }
    }
}