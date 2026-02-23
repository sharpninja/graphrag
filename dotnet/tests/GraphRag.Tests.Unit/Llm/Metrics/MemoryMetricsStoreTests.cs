// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Metrics;

namespace GraphRag.Tests.Unit.Llm.Metrics;

/// <summary>
/// Unit tests for <see cref="MemoryMetricsStore"/>.
/// </summary>
public class MemoryMetricsStoreTests
{
    [Fact]
    public void Record_StoresMetrics()
    {
        var sut = new MemoryMetricsStore();
        var metrics = new Dictionary<string, double> { ["latency"] = 1.5 };

        sut.Record("op1", metrics);

        sut.GetAll().Should().ContainKey("op1");
        sut.GetAll()["op1"]["latency"].Should().Be(1.5);
    }

    [Fact]
    public void GetAll_ReturnsAllRecorded()
    {
        var sut = new MemoryMetricsStore();
        sut.Record("a", new Dictionary<string, double> { ["x"] = 1.0 });
        sut.Record("b", new Dictionary<string, double> { ["y"] = 2.0 });

        var all = sut.GetAll();

        all.Should().HaveCount(2);
        all.Should().ContainKey("a");
        all.Should().ContainKey("b");
    }
}
