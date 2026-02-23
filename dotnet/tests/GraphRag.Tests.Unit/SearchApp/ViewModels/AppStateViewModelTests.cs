// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.SearchApp.Config;
using GraphRag.SearchApp.Models;
using GraphRag.SearchApp.ViewModels;

namespace GraphRag.Tests.Unit.SearchApp.ViewModels;

/// <summary>
/// Tests for <see cref="AppStateViewModel"/>.
/// </summary>
public class AppStateViewModelTests
{
    [Fact]
    public void DatasetKey_SetValue_RaisesPropertyChanged()
    {
        var vm = new AppStateViewModel();
        var raised = false;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(AppStateViewModel.DatasetKey))
            {
                raised = true;
            }
        };

        vm.DatasetKey = "test-key";

        raised.Should().BeTrue();
        vm.DatasetKey.Should().Be("test-key");
    }

    [Fact]
    public void DatasetKey_SetSameValue_DoesNotRaisePropertyChanged()
    {
        var vm = new AppStateViewModel();
        vm.DatasetKey = "test";
        var raised = false;
        vm.PropertyChanged += (_, _) => raised = true;

        vm.DatasetKey = "test";

        raised.Should().BeFalse();
    }

    [Fact]
    public void Datasets_IsObservableCollection()
    {
        var vm = new AppStateViewModel();
        vm.Datasets.Should().NotBeNull().And.BeEmpty();

        var ds = new DatasetConfig("k1", "/path", "Test", "Desc", 0);
        vm.Datasets.Add(ds);

        vm.Datasets.Should().ContainSingle().Which.Key.Should().Be("k1");
    }

    [Fact]
    public void GeneratedQuestions_IsObservableCollection()
    {
        var vm = new AppStateViewModel();
        vm.GeneratedQuestions.Should().NotBeNull().And.BeEmpty();

        vm.GeneratedQuestions.Add("What is GraphRAG?");
        vm.GeneratedQuestions.Should().ContainSingle();
    }

    [Fact]
    public void GetEnabledSearchTypes_DefaultConfig_ReturnsGlobalAndLocal()
    {
        var vm = new AppStateViewModel();
        var types = vm.GetEnabledSearchTypes();

        types.Should().HaveCount(2);
        types.Should().Contain(SearchType.Global);
        types.Should().Contain(SearchType.Local);
    }

    [Fact]
    public void GetEnabledSearchTypes_AllEnabled_ReturnsFourTypes()
    {
        var vm = new AppStateViewModel
        {
            IncludeGlobalSearch = true,
            IncludeLocalSearch = true,
            IncludeDriftSearch = true,
            IncludeBasicRag = true,
        };

        var types = vm.GetEnabledSearchTypes();
        types.Should().HaveCount(4);
    }

    [Fact]
    public void GetEnabledSearchTypes_NoneEnabled_ReturnsEmpty()
    {
        var vm = new AppStateViewModel
        {
            IncludeGlobalSearch = false,
            IncludeLocalSearch = false,
        };

        vm.GetEnabledSearchTypes().Should().BeEmpty();
    }

    [Fact]
    public void IsLoading_DefaultIsFalse()
    {
        var vm = new AppStateViewModel();
        vm.IsLoading.Should().BeFalse();
    }

    [Fact]
    public void SuggestedQuestionsCount_DefaultIsFive()
    {
        var vm = new AppStateViewModel();
        vm.SuggestedQuestionsCount.Should().Be(5);
    }

    [Fact]
    public void KnowledgeModel_SetRaisesPropertyChanged()
    {
        var vm = new AppStateViewModel();
        var raised = false;
        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(AppStateViewModel.KnowledgeModel))
            {
                raised = true;
            }
        };

        vm.KnowledgeModel = new KnowledgeModel();
        raised.Should().BeTrue();
    }
}
