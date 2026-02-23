// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.DataModel;
using GraphRag.SearchApp.ViewModels;

namespace GraphRag.Tests.Unit.SearchApp.ViewModels;

/// <summary>
/// Tests for <see cref="CommunityExplorerViewModel"/>.
/// </summary>
public class CommunityExplorerViewModelTests
{
    [Fact]
    public void Reports_IsObservableCollection()
    {
        var vm = new CommunityExplorerViewModel();
        vm.Reports.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void SelectedReport_SetValue_RaisesPropertyChanged()
    {
        var vm = new CommunityExplorerViewModel();
        var raised = false;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CommunityExplorerViewModel.SelectedReport))
            {
                raised = true;
            }
        };

        var report = new CommunityReport { Id = "r1", Title = "Report 1", CommunityId = "1" };
        vm.SelectedReport = report;

        raised.Should().BeTrue();
        vm.SelectedReport.Should().Be(report);
    }

    [Fact]
    public void FilterLevel_SetValue_RaisesPropertyChanged()
    {
        var vm = new CommunityExplorerViewModel();
        var raised = false;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CommunityExplorerViewModel.FilterLevel))
            {
                raised = true;
            }
        };

        vm.FilterLevel = 2;

        raised.Should().BeTrue();
        vm.FilterLevel.Should().Be(2);
    }

    [Fact]
    public void GetFilteredReports_NoFilter_ReturnsAll()
    {
        var vm = new CommunityExplorerViewModel();
        vm.Reports.Add(new CommunityReport { Id = "r1", Title = "R1", CommunityId = "1-0" });
        vm.Reports.Add(new CommunityReport { Id = "r2", Title = "R2", CommunityId = "2-0" });

        var filtered = vm.GetFilteredReports();
        filtered.Should().HaveCount(2);
    }

    [Fact]
    public void GetFilteredReports_WithFilter_ReturnsMatching()
    {
        var vm = new CommunityExplorerViewModel();
        vm.Reports.Add(new CommunityReport { Id = "r1", Title = "R1", CommunityId = "1-0" });
        vm.Reports.Add(new CommunityReport { Id = "r2", Title = "R2", CommunityId = "2-0" });
        vm.Reports.Add(new CommunityReport { Id = "r3", Title = "R3", CommunityId = "1-1" });

        vm.FilterLevel = 1;
        var filtered = vm.GetFilteredReports();

        filtered.Should().HaveCount(2);
        filtered.Should().OnlyContain(r => r.CommunityId.StartsWith('1'));
    }
}
