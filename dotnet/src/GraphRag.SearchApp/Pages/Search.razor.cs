// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;
using GraphRag.SearchApp.Models;
using Microsoft.AspNetCore.Components.Web;

namespace GraphRag.SearchApp.Pages;

/// <summary>
/// Code-behind for the main Search page (includes tab-based Community Explorer).
/// </summary>
public partial class Search
{
    private int _activeTab;
    private bool _isSuggesting;
    private List<SearchType> _activeSearchTypes = [];

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LoadReports();
    }

    private static string GetSearchTypeLabel(SearchType type) => type switch
    {
        SearchType.Basic => "basic RAG",
        SearchType.Local => "local search",
        SearchType.Global => "global search",
        SearchType.Drift => "drift search",
        _ => type.ToString(),
    };

    private int GetColumnWidth()
    {
        var count = SearchVm.Results.Count;
        if (count == 0)
        {
            return 12;
        }

        return 12 / count;
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

    private async Task SuggestQuestions()
    {
        _isSuggesting = true;
        StateHasChanged();

        try
        {
            // Placeholder — real implementation would call global search for suggestions
            await Task.Delay(500).ConfigureAwait(false);
            AppState.GeneratedQuestions.Clear();

            // In production, this calls the search engine to generate questions
            AppState.GeneratedQuestions.Add("What are the main topics covered in this dataset?");
            AppState.GeneratedQuestions.Add("What are the key entities and relationships?");
            AppState.GeneratedQuestions.Add("What are the most important community findings?");
        }
        finally
        {
            _isSuggesting = false;
            StateHasChanged();
        }
    }

    private void ResetQuestions()
    {
        AppState.GeneratedQuestions.Clear();
    }

    private async Task ExecuteSearch()
    {
        if (string.IsNullOrWhiteSpace(AppState.Question))
        {
            return;
        }

        SearchVm.Clear();
        SearchVm.IsSearching = true;
        _activeSearchTypes = [.. AppState.GetEnabledSearchTypes()];
        StateHasChanged();

        try
        {
            var enabledTypes = AppState.GetEnabledSearchTypes();
            if (enabledTypes.Count == 0)
            {
                SearchVm.SearchError = "No search types enabled. Toggle at least one in the sidebar.";
                return;
            }

            // Build engines from factory — placeholder until full wiring
            var engines = new Dictionary<SearchType, GraphRag.Query.ISearch>();

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
            _activeSearchTypes = [];
            StateHasChanged();
        }
    }

    private void OnReportSelected(CommunityReport report)
    {
        ExplorerVm.SelectedReport = report;
    }

    private void LoadReports()
    {
        if (AppState.KnowledgeModel is null)
        {
            return;
        }

        ExplorerVm.Reports.Clear();
        foreach (var report in AppState.KnowledgeModel.CommunityReports)
        {
            ExplorerVm.Reports.Add(report);
        }
    }
}
