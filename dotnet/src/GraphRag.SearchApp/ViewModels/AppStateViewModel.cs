// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.ObjectModel;
using GraphRag.SearchApp.Config;
using GraphRag.SearchApp.Models;

namespace GraphRag.SearchApp.ViewModels;

/// <summary>
/// Main application state ViewModel, scoped per Blazor circuit.
/// </summary>
public class AppStateViewModel : ViewModelBase
{
    private string _datasetKey = string.Empty;
    private DatasetConfig? _datasetConfig;
    private KnowledgeModel? _knowledgeModel;
    private string _question = string.Empty;
    private bool _isLoading;
    private bool _includeGlobalSearch = true;
    private bool _includeLocalSearch = true;
    private bool _includeDriftSearch;
    private bool _includeBasicRag;
    private int _suggestedQuestionsCount = 5;

    /// <summary>
    /// Gets the available datasets.
    /// </summary>
    public ObservableCollection<DatasetConfig> Datasets { get; } = [];

    /// <summary>
    /// Gets the generated questions.
    /// </summary>
    public ObservableCollection<string> GeneratedQuestions { get; } = [];

    /// <summary>
    /// Gets or sets the currently selected dataset key.
    /// </summary>
    public string DatasetKey
    {
        get => _datasetKey;
        set => SetField(ref _datasetKey, value);
    }

    /// <summary>
    /// Gets or sets the current dataset configuration.
    /// </summary>
    public DatasetConfig? DatasetConfig
    {
        get => _datasetConfig;
        set => SetField(ref _datasetConfig, value);
    }

    /// <summary>
    /// Gets or sets the loaded knowledge model.
    /// </summary>
    public KnowledgeModel? KnowledgeModel
    {
        get => _knowledgeModel;
        set => SetField(ref _knowledgeModel, value);
    }

    /// <summary>
    /// Gets or sets the current search question.
    /// </summary>
    public string Question
    {
        get => _question;
        set => SetField(ref _question, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the app is loading data.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetField(ref _isLoading, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether global search is enabled.
    /// </summary>
    public bool IncludeGlobalSearch
    {
        get => _includeGlobalSearch;
        set => SetField(ref _includeGlobalSearch, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether local search is enabled.
    /// </summary>
    public bool IncludeLocalSearch
    {
        get => _includeLocalSearch;
        set => SetField(ref _includeLocalSearch, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether DRIFT search is enabled.
    /// </summary>
    public bool IncludeDriftSearch
    {
        get => _includeDriftSearch;
        set => SetField(ref _includeDriftSearch, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether basic RAG search is enabled.
    /// </summary>
    public bool IncludeBasicRag
    {
        get => _includeBasicRag;
        set => SetField(ref _includeBasicRag, value);
    }

    /// <summary>
    /// Gets or sets the number of suggested questions to generate.
    /// </summary>
    public int SuggestedQuestionsCount
    {
        get => _suggestedQuestionsCount;
        set => SetField(ref _suggestedQuestionsCount, value);
    }

    /// <summary>
    /// Gets the list of enabled search types.
    /// </summary>
    /// <returns>A list of currently enabled search types.</returns>
    public IReadOnlyList<SearchType> GetEnabledSearchTypes()
    {
        var types = new List<SearchType>();
        if (IncludeGlobalSearch)
        {
            types.Add(SearchType.Global);
        }

        if (IncludeLocalSearch)
        {
            types.Add(SearchType.Local);
        }

        if (IncludeDriftSearch)
        {
            types.Add(SearchType.Drift);
        }

        if (IncludeBasicRag)
        {
            types.Add(SearchType.Basic);
        }

        return types;
    }
}
