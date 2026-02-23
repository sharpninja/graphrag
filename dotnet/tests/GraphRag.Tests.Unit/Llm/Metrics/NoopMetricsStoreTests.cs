// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Metrics;

namespace GraphRag.Tests.Unit.Llm.Metrics;

/// <summary>
/// Unit tests for <see cref="NoopMetricsStore"/>.
/// </summary>
public class NoopMetricsStoreTests
{
    [Fact]
    public void Record_DoesNotThrow()
    {
        var sut = new NoopMetricsStore();

        var act = () => sut.Record("key", new Dictionary<string, double> { ["val"] = 1.0 });

        act.Should().NotThrow();
    }

    [Fact]
    public void GetAll_ReturnsEmpty()
    {
        var sut = new NoopMetricsStore();
        sut.Record("key", new Dictionary<string, double> { ["val"] = 1.0 });

        var all = sut.GetAll();

        all.Should().BeEmpty();
    }
}
