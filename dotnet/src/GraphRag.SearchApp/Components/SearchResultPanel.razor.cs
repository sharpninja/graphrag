// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.SearchApp.Models;
using Microsoft.AspNetCore.Components;

namespace GraphRag.SearchApp.Components;

/// <summary>
/// Displays a single search result with stats and formatted response.
/// </summary>
public partial class SearchResultPanel
{
    /// <summary>
    /// Gets or sets the search result to display.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public AppSearchResult Result { get; set; } = default!;

    private string GetTypeIcon()
    {
        return Result.Type switch
        {
            SearchType.Global => MudBlazor.Icons.Material.Filled.Public,
            SearchType.Local => MudBlazor.Icons.Material.Filled.Place,
            SearchType.Drift => MudBlazor.Icons.Material.Filled.Explore,
            SearchType.Basic => MudBlazor.Icons.Material.Filled.TextSnippet,
            _ => MudBlazor.Icons.Material.Filled.QuestionMark,
        };
    }
}
