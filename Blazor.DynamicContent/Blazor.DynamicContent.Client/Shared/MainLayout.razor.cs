using MudBlazor;

namespace Blazor.DynamicContent.Client.Shared
{
    public partial class MainLayout
    {
        private MudTheme _theme = new MudTheme
        {
            Palette = new Palette
            {
                AppbarText = "#ff584f",
                AppbarBackground = "#ffffff",
                Primary = "#3d6fb4",
                Secondary =  "#ff584f",
            }
        };
        private bool _isDarkMode;
        private bool _drawerOpen;

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}