// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;
using GraphRag.SearchApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace GraphRag.SearchApp.Pages;

/// <summary>
/// Code-behind for the main Search page (includes tab-based Community Explorer).
/// </summary>
public partial class Search : IDisposable
{
    private int _activeTab;
    private bool _isSuggesting;
    private List<SearchType> _activeSearchTypes = [];

    /// <inheritdoc />
    public void Dispose()
    {
        AppState.PropertyChanged -= OnAppStateChanged;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        AppState.PropertyChanged += OnAppStateChanged;
        await LoadDatasetsAsync().ConfigureAwait(false);
    }

    private static string GetSearchTypeLabel(SearchType type) => type switch
    {
        SearchType.Basic => "basic RAG",
        SearchType.Local => "local search",
        SearchType.Global => "global search",
        SearchType.Drift => "drift search",
        _ => type.ToString(),
    };

    private async Task LoadDatasetsAsync()
    {
        var datasets = await Loader.LoadDatasetListingAsync().ConfigureAwait(false);
        AppState.Datasets.Clear();
        foreach (var ds in datasets)
        {
            AppState.Datasets.Add(ds);
        }

        if (AppState.Datasets.Count > 0 && string.IsNullOrEmpty(AppState.DatasetKey))
        {
            AppState.DatasetKey = AppState.Datasets[0].Key;
        }
    }

    private async void OnAppStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppState.DatasetKey))
        {
            await LoadDatasetAsync().ConfigureAwait(false);
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }
    }

    private async Task LoadDatasetAsync()
    {
        var ds = AppState.Datasets.FirstOrDefault(d => d.Key == AppState.DatasetKey);
        if (ds is null)
        {
            return;
        }

        AppState.DatasetConfig = ds;
        AppState.IsLoading = true;

        try
        {
            var datasource = Loader.CreateDatasource(ds);
            AppState.KnowledgeModel = await ModelService.LoadModelAsync(datasource).ConfigureAwait(false);
            LoadReports();
        }
        catch (Exception ex)
        {
            SearchVm.SearchError = $"Failed to load dataset: {ex.Message}";
        }
        finally
        {
            AppState.IsLoading = false;
        }
    }

    private void SetActiveTab(int tab)
    {
        _activeTab = tab;
    }

    private void OnQuestionInput(ChangeEventArgs e)
    {
        AppState.Question = e.Value?.ToString() ?? string.Empty;
    }

    private int GetEnabledCount()
    {
        return AppState.GetEnabledSearchTypes().Count;
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
