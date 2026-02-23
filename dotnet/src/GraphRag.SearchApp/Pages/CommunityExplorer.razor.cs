// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;

namespace GraphRag.SearchApp.Pages;

/// <summary>
/// Code-behind for the Community Explorer page.
/// </summary>
public partial class CommunityExplorer
{
    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LoadReports();
    }

    private IEnumerable<int> GetCommunityLevels()
    {
        return ExplorerVm.Reports
            .Select(r => int.TryParse(r.CommunityId.Split('-').FirstOrDefault(), out var level) ? level : 0)
            .Distinct()
            .OrderBy(l => l);
    }

    private void OnFilterChanged(int? level)
    {
        ExplorerVm.FilterLevel = level;
    }

    private void OnReportSelected(CommunityReport? report)
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
