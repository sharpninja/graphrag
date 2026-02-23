// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.SearchApp.Models;
using GraphRag.SearchApp.ViewModels;

namespace GraphRag.Tests.Unit.SearchApp.ViewModels;

/// <summary>
/// Tests for <see cref="SearchViewModel"/>.
/// </summary>
public class SearchViewModelTests
{
    [Fact]
    public void Results_IsObservableCollection()
    {
        var vm = new SearchViewModel();
        vm.Results.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void IsSearching_SetValue_RaisesPropertyChanged()
    {
        var vm = new SearchViewModel();
        var raised = false;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SearchViewModel.IsSearching))
            {
                raised = true;
            }
        };

        vm.IsSearching = true;

        raised.Should().BeTrue();
        vm.IsSearching.Should().BeTrue();
    }

    [Fact]
    public void SearchError_SetValue_RaisesPropertyChanged()
    {
        var vm = new SearchViewModel();
        var raised = false;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SearchViewModel.SearchError))
            {
                raised = true;
            }
        };

        vm.SearchError = "Test error";

        raised.Should().BeTrue();
        vm.SearchError.Should().Be("Test error");
    }

    [Fact]
    public void Clear_RemovesResultsAndError()
    {
        var vm = new SearchViewModel
        {
            SearchError = "Some error",
        };
        vm.Results.Add(new AppSearchResult(SearchType.Global, "response"));

        vm.Clear();

        vm.Results.Should().BeEmpty();
        vm.SearchError.Should().BeNull();
    }

    [Fact]
    public void Results_AddMultiple_TracksAll()
    {
        var vm = new SearchViewModel();
        vm.Results.Add(new AppSearchResult(SearchType.Global, "global result"));
        vm.Results.Add(new AppSearchResult(SearchType.Local, "local result"));

        vm.Results.Should().HaveCount(2);
        vm.Results[0].Type.Should().Be(SearchType.Global);
        vm.Results[1].Type.Should().Be(SearchType.Local);
    }
}
