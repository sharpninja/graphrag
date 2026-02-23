// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.ObjectModel;
using GraphRag.SearchApp.Models;

namespace GraphRag.SearchApp.ViewModels;

/// <summary>
/// ViewModel for the search page.
/// </summary>
public class SearchViewModel : ViewModelBase
{
    private bool _isSearching;
    private string? _searchError;

    /// <summary>
    /// Gets the search results.
    /// </summary>
    public ObservableCollection<AppSearchResult> Results { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether a search is in progress.
    /// </summary>
    public bool IsSearching
    {
        get => _isSearching;
        set => SetField(ref _isSearching, value);
    }

    /// <summary>
    /// Gets or sets the most recent search error message.
    /// </summary>
    public string? SearchError
    {
        get => _searchError;
        set => SetField(ref _searchError, value);
    }

    /// <summary>
    /// Clears all search results and resets error state.
    /// </summary>
    public void Clear()
    {
        Results.Clear();
        SearchError = null;
    }
}
