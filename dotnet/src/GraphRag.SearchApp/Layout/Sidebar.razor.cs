// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.SearchApp.Layout;

/// <summary>
/// Code-behind for the sidebar component.
/// </summary>
public partial class Sidebar
{
    private async Task OnDatasetChanged(string newKey)
    {
        if (AppState is not null && newKey != AppState.DatasetKey)
        {
            AppState.DatasetKey = newKey;
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
