// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.ObjectModel;
using GraphRag.DataModel;

namespace GraphRag.SearchApp.ViewModels;

/// <summary>
/// ViewModel for the community explorer page.
/// </summary>
public class CommunityExplorerViewModel : ViewModelBase
{
    private CommunityReport? _selectedReport;
    private int? _filterLevel;

    /// <summary>
    /// Gets the community reports.
    /// </summary>
    public ObservableCollection<CommunityReport> Reports { get; } = [];

    /// <summary>
    /// Gets or sets the currently selected community report.
    /// </summary>
    public CommunityReport? SelectedReport
    {
        get => _selectedReport;
        set => SetField(ref _selectedReport, value);
    }

    /// <summary>
    /// Gets or sets the community level filter.
    /// </summary>
    public int? FilterLevel
    {
        get => _filterLevel;
        set => SetField(ref _filterLevel, value);
    }

    /// <summary>
    /// Gets the filtered list of reports based on the current level filter.
    /// </summary>
    /// <returns>The filtered reports.</returns>
    public IReadOnlyList<CommunityReport> GetFilteredReports()
    {
        if (FilterLevel is null)
        {
            return [.. Reports];
        }

        return Reports
            .Where(r => r.CommunityId.StartsWith(FilterLevel.Value.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal))
            .ToList();
    }
}
