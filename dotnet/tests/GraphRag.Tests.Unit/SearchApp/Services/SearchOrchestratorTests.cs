// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Query;
using GraphRag.SearchApp.Models;
using GraphRag.SearchApp.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace GraphRag.Tests.Unit.SearchApp.Services;

/// <summary>
/// Tests for <see cref="SearchOrchestrator"/>.
/// </summary>
public class SearchOrchestratorTests
{
    [Fact]
    public async Task RunSearchesAsync_EmptyEngines_ReturnsEmpty()
    {
        var orchestrator = new SearchOrchestrator(NullLogger<SearchOrchestrator>.Instance);
        var engines = new Dictionary<SearchType, ISearch>();

        var results = await orchestrator.RunSearchesAsync("test query", engines);

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task RunSearchesAsync_SingleEngine_ReturnsSingleResult()
    {
        var orchestrator = new SearchOrchestrator(NullLogger<SearchOrchestrator>.Instance);
        var mockSearch = new Mock<ISearch>();
        mockSearch.Setup(s => s.SearchAsync("test", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchResult("global response", CompletionTime: 1.5, LlmCalls: 2));

        var engines = new Dictionary<SearchType, ISearch>
        {
            { SearchType.Global, mockSearch.Object },
        };

        var results = await orchestrator.RunSearchesAsync("test", engines);

        results.Should().ContainSingle();
        results[0].Type.Should().Be(SearchType.Global);
        results[0].Response.Should().Be("global response");
        results[0].LlmCalls.Should().Be(2);
    }

    [Fact]
    public async Task RunSearchesAsync_MultipleEngines_RunsInParallel()
    {
        var orchestrator = new SearchOrchestrator(NullLogger<SearchOrchestrator>.Instance);
        var globalSearch = new Mock<ISearch>();
        var localSearch = new Mock<ISearch>();

        globalSearch.Setup(s => s.SearchAsync("q", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchResult("global"));
        localSearch.Setup(s => s.SearchAsync("q", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SearchResult("local"));

        var engines = new Dictionary<SearchType, ISearch>
        {
            { SearchType.Global, globalSearch.Object },
            { SearchType.Local, localSearch.Object },
        };

        var results = await orchestrator.RunSearchesAsync("q", engines);

        results.Should().HaveCount(2);
        results.Should().Contain(r => r.Type == SearchType.Global);
        results.Should().Contain(r => r.Type == SearchType.Local);
    }

    [Fact]
    public async Task RunSearchesAsync_EngineThrows_ReturnsErrorResult()
    {
        var orchestrator = new SearchOrchestrator(NullLogger<SearchOrchestrator>.Instance);
        var mockSearch = new Mock<ISearch>();
        mockSearch.Setup(s => s.SearchAsync("q", null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("LLM failure"));

        var engines = new Dictionary<SearchType, ISearch>
        {
            { SearchType.Basic, mockSearch.Object },
        };

        var results = await orchestrator.RunSearchesAsync("q", engines);

        results.Should().ContainSingle();
        results[0].Type.Should().Be(SearchType.Basic);
        results[0].Response.Should().Contain("Error: LLM failure");
    }

    [Fact]
    public async Task RunSearchesAsync_NullQuery_ThrowsArgumentException()
    {
        var orchestrator = new SearchOrchestrator(NullLogger<SearchOrchestrator>.Instance);
        var engines = new Dictionary<SearchType, ISearch>();

        var act = () => orchestrator.RunSearchesAsync(string.Empty, engines);

        await act.Should().ThrowAsync<ArgumentException>();
    }
}
