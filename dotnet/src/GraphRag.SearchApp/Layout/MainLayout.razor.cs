// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using MudBlazor;

namespace GraphRag.SearchApp.Layout;

/// <summary>
/// Code-behind for the main layout component.
/// </summary>
public partial class MainLayout
{
    private readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#ff4b4b",
            AppbarBackground = "#ffffff",
            AppbarText = "#424242",
            DrawerBackground = "#ffffff",
        },
    };

    private bool _drawerOpen = true;

    private void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }
}
