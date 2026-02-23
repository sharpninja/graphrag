// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.AspNetCore.Components.Web;

namespace GraphRag.SearchApp.Pages;

/// <summary>
/// Code-behind for the Search page.
/// </summary>
public partial class Search
{
    private int GetColumnWidth()
    {
        var count = SearchVm.Results.Count;
        return count switch
        {
            1 => 12,
            2 => 6,
            3 => 4,
            _ => 3,
        };
    }

    private async Task OnSearchKeyUp(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await ExecuteSearch().ConfigureAwait(false);
        }
    }

    private async Task OnQuestionSelected(string question)
    {
        AppState.Question = question;
        await ExecuteSearch().ConfigureAwait(false);
    }

    private async Task ExecuteSearch()
    {
        if (string.IsNullOrWhiteSpace(AppState.Question))
        {
            return;
        }

        SearchVm.Clear();
        SearchVm.IsSearching = true;

        try
        {
            var enabledTypes = AppState.GetEnabledSearchTypes();
            if (enabledTypes.Count == 0)
            {
                SearchVm.SearchError = "No search types enabled. Toggle at least one in the sidebar.";
                return;
            }

            // Build engines from factory — placeholder until full wiring in B7
            var engines = new Dictionary<Models.SearchType, GraphRag.Query.ISearch>();

            var results = await Orchestrator.RunSearchesAsync(
                AppState.Question,
                engines).ConfigureAwait(false);

            foreach (var r in results)
            {
                SearchVm.Results.Add(r);
            }
        }
        catch (Exception ex)
        {
            SearchVm.SearchError = $"Search failed: {ex.Message}";
        }
        finally
        {
            SearchVm.IsSearching = false;
        }
    }
}
